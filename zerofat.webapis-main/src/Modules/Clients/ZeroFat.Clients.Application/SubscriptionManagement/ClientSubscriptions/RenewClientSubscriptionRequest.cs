using Mapster;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.Domain.Common;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.ClientSubscriptions;

public class RenewClientSubscriptionRequest : ICommand<Result<DefaultIdType>>
{
    public string? PaymentMethodId { get; set; }
}

public class RenewClientSubscriptionRequestValidator : CustomValidator<RenewClientSubscriptionRequest>
{
    public RenewClientSubscriptionRequestValidator(
        ICurrentUser currentUser,
        IStringLocalizer<RenewClientSubscriptionRequestValidator> localizer)
    {
        RuleFor(x => currentUser.GetRoleType())
            .Must(role => role != null && role.Equals(nameof(UserType.Client), StringComparison.OrdinalIgnoreCase))
            .WithMessage(localizer["Only clients can renew subscriptions."]);
    }
}


public class RenewClientSubscriptionRequestHandler(
    IRepositoryWithEvents<ClientSubscription> subscriptionRepo,
    IRepository<Client> clientRepo,
    IReadRepository<MealPlanMealType> mealPlanMealTypeRepo,
    IStripeService stripeService,
    IRepositoryWithEvents<Payment> paymentRepo,
    IRepositoryWithEvents<ClientLoyaltyPoint> clientLoyaltyPointRepo,
    IClientPortalSettingservice clientPortalSettingservice,
    IStringLocalizer<RenewClientSubscriptionRequestHandler> localizer,
    ILogger<RenewClientSubscriptionRequestHandler> logger,
    IJobService jobService,
    ICurrentUser currentUser)
    : ICommandHandler<RenewClientSubscriptionRequest, Result<DefaultIdType>>
{
    public async Task<Result<DefaultIdType>> Handle(RenewClientSubscriptionRequest request, CancellationToken cancellationToken)
    {
        var client = await clientRepo.FirstOrDefaultAsync(
            new ExpressionSpecification<Client>(x => x.Id == currentUser.GetUserId()), cancellationToken);
        _ = client ?? throw new NotFoundException(localizer["Client not found"]);

        if (client.ClientSubscriptionId == null)
            throw new ForbiddenException(localizer["Cannot renew inactive subscription"]);

        var oldSub = await subscriptionRepo.FirstOrDefaultAsync(
            new ClientSubscriptionByIdSpec(client.ClientSubscriptionId.Value),
            cancellationToken);
        _ = oldSub ?? throw new NotFoundException(localizer["Subscription not found"]);

        if (client.SubscriptionStatus != SubscriptionStatus.Active)
            throw new ForbiddenException(localizer["Cannot renew inactive subscription"]);

        // 3.Create a New Subscription record for the renewal period
        var newSubscription = new ClientSubscription
        {
            ClientId = client.Id,
            MealPlanId = oldSub.MealPlanId,
            ClientLocationId = oldSub.ClientLocationId,
            SubscriptionType = oldSub.SubscriptionType, // Use the type from the request
            SelectedDeliveryDays = oldSub.SelectedDeliveryDays,
            PreferredDeliveryTime = oldSub.PreferredDeliveryTime,
            IsAutoRenewalEnabled = oldSub.IsAutoRenewalEnabled, // Preserve auto-renewal setting
            RenewalCount = oldSub.RenewalCount + 1,
            PaymentStatus = PaymentStatus.Pending,
            SubscriptionStatus = SubscriptionStatus.Pending,
            // Deep copy meal selections to avoid EF tracking issues
            SelectedMealTypes = oldSub.SelectedMealTypes.Select(s => new MealTypeSelection
            {
                QuantityPerDay = s.QuantityPerDay,
                MealTypeId = s.MealTypeId,
                Price = s.Price,
                MealTypeNameAr = s.MealTypeNameAr,
                MealTypeNameEn = s.MealTypeNameEn,
            }).ToList()
        };

        // 4. Calculate Dates, Cost, and Calories
        newSubscription.StartDate = oldSub.EndDate.AddDays(1);
        newSubscription.AverageCalories = oldSub.AverageCalories;
        newSubscription.TotalCost = oldSub.SelectedMealTypes.Sum(s => s.QuantityPerDay * s.Price);
        newSubscription.TotalCost *= newSubscription.SelectedDeliveryDays.Count;

        // Apply package type rules
        switch (oldSub.SubscriptionType)
        {
            case SubscriptionType.OneWeek:
                newSubscription.EndDate = newSubscription.StartDate.AddDays(6);
                break;
            case SubscriptionType.OneMonth:
                newSubscription.EndDate = newSubscription.StartDate.AddDays(27);
                newSubscription.TotalCost *= 4;

                var monthlyDiscount = await clientPortalSettingservice.GetMonthlyPackageSubsciptionDiscount();
                if (monthlyDiscount > 0)
                    newSubscription.TotalCost -= newSubscription.TotalCost * monthlyDiscount / 100;
                break;
            case SubscriptionType.ThreeMonths:
                newSubscription.EndDate = newSubscription.StartDate.AddDays((28 * 3) - 1);
                newSubscription.TotalCost *= 12;

                var threeMonthlyDiscount = await clientPortalSettingservice.GetThreeMonthlyPackageSubsciptionDiscount();
                if (threeMonthlyDiscount > 0)
                    newSubscription.TotalCost -= newSubscription.TotalCost * threeMonthlyDiscount / 100;
                break;
        }

        // 5. Process Immediate Payment with Stripe
        var description = $"Renewal for {oldSub.SubscriptionType} Subscription";
        var paymentIntent = await stripeService.CreateAddOnPaymentLink(
            newSubscription.Id.ToString(),
            request.PaymentMethodId,
            client.StripeId!,
            newSubscription.TotalCost,
            description);

        if (paymentIntent?.Status != "succeeded")
        {
            logger.LogWarning("Payment failed for renewal of subscription {SubscriptionId} with status {Status}",
                oldSub.Id, paymentIntent?.Status);
            throw new BadRequestException(localizer["Payment processing failed. Please check your payment details and try again."]);
        }

        logger.LogInformation("Payment successful for subscription renewal {SubscriptionId}", newSubscription.Id);

        // 6. Update Entities and Create Related Records
        newSubscription.PaymentStatus = PaymentStatus.Paid;
        newSubscription.SubscriptionStatus = SubscriptionStatus.Active;
        newSubscription.PaymentDate = SystemTime.Now();
        newSubscription.StripeSubscriptionId = paymentIntent.Id; // Storing PaymentIntent ID for reference
        var offset = await clientPortalSettingservice.GetOffsetSubscriptionInDays();
        newSubscription.NextRenewalDate = newSubscription.EndDate.AddDays(1 - offset);

        client.ClientSubscriptionId = newSubscription.Id;
        client.SubscriptionStatus = SubscriptionStatus.Active;

        await subscriptionRepo.AddAsync(newSubscription, withSaveChanges: false, cancellationToken: cancellationToken);

        var payment = CreatePaymentRecord(client, newSubscription, paymentIntent, description);
        await paymentRepo.AddAsync(payment, withSaveChanges: false, cancellationToken: cancellationToken);

        await AddLoyaltyPoints(client, payment, description, cancellationToken);

        // 7. Save to Database
        await subscriptionRepo.SaveChangesAsync(cancellationToken);

        // 8. Enqueue background jobs
        jobService.Enqueue<ISubscriptionMealOrchestrator>(x => x.CreateDailySelectionsForSubscriptionAsync(newSubscription.Id));

        return await Result<DefaultIdType>.SuccessAsync(newSubscription.Id);
    }


    private Payment CreatePaymentRecord(Client client, ClientSubscription subscription, PaymentIntentResponseDTO paymentIntent, string reason)
    {
        logger.LogDebug("Creating payment record for payment intent {PaymentIntentId}", paymentIntent.Id);
        return new Payment
        {
            ClientId = client.Id,
            ClientSubscriptionId = subscription.Id,
            InvoiceNumber = paymentIntent.Id,
            TransactionId = paymentIntent.LatestChargeId ?? paymentIntent.Id,
            PaymentStatus = PaymentStatus.Paid,
            PaymentMethod = paymentIntent.PaymentMethodType ?? "Card",
            Amount = paymentIntent.Amount / 100m,
            PaymentGateway = "Stripe",
            Reason = reason,
            Currency = paymentIntent.Currency,
            PaymentDate = SystemTime.Now(),
            OrderId = paymentIntent.OrderId
        };
    }

    private async Task AddLoyaltyPoints(Client client, Payment payment, string reason, CancellationToken cancellationToken)
    {
        logger.LogDebug("Calculating loyalty points for payment {PaymentId}", payment.Id);

        var pointsPerMoney = await clientPortalSettingservice.GetOnePointEqualInMoney();
        var pointsEarned = pointsPerMoney == 0 ? 0 : (int)(payment.Amount / pointsPerMoney);

        if (pointsEarned > 0)
        {
            await clientLoyaltyPointRepo.AddAsync(new ClientLoyaltyPoint
            {
                ClientId = client.Id,
                Reason = reason,
                Source = "Payment",
                TransactionId = payment.Id.ToString(),
                Points = pointsEarned,
                Type = TransactionType.Earn,
                DateEarned = SystemTime.Now(),
            }, withSaveChanges: false, cancellationToken: cancellationToken);

            logger.LogInformation("Awarded {PointsEarned} loyalty points to client {ClientId}", pointsEarned, client.Id);
        }
    }
}
