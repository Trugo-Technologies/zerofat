using Ardalis.Specification;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.DailyMealSelections;
public class CustomizeDailyMealSelectionRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType? Id { get; set; }
    public List<DefaultIdType> SelectedOptionIds { get; set; } = [];
    public string? CustomeMealName { get; set; }
    public string? PaymentMethodId { get; set; }
    public bool SaveCustomeMeal { get; set; }
}

public class CustomizeDailyMealSelectionRequestHandler(
    IRepositoryWithEvents<DailyMealSelection> repository,
    IRepositoryWithEvents<DailySelection> dailySelectionRepo,
    IRepositoryWithEvents<CustomMeal> customMealRepo,
    IRepositoryWithEvents<Client> clientRepo,
    IRepositoryWithEvents<Payment> paymentRepo,
    IRepositoryWithEvents<ClientLoyaltyPoint> clientLoyaltyPointRepo,
    IRepositoryWithEvents<MealCustomizationOption> customizationOptionRepo,
    IStripeService stripeService,
    IClientPortalSettingservice clientPortalSettingsService,
    ILogger<CustomizeDailyMealSelectionRequestHandler> logger,
    ICurrentUser currentUser,
    IStringLocalizer<CustomizeDailyMealSelectionRequestHandler> localizer) : IRequestHandler<CustomizeDailyMealSelectionRequest, Result<DefaultIdType>>
{


    public async Task<Result<DefaultIdType>> Handle(CustomizeDailyMealSelectionRequest request, CancellationToken cancellationToken)
    {
        bool isClient = currentUser.GetRoleType()!.Equals(nameof(UserType.Client).ToString(), StringComparison.OrdinalIgnoreCase);
        if (!isClient)
        {
            throw new BadRequestException("only client can register");
        }

        DailyMealSelection? entity = await repository.GetByIdAsync(request.Id!.Value, cancellationToken);
        _ = entity ?? throw new NotFoundException(localizer["DailyMealSelection not found"]);

        var dailySelection = await dailySelectionRepo.GetByIdAsync(entity.DailySelectionId, cancellationToken)
               ?? throw new NotFoundException("Your daily menu couldn't be found. Please refresh and try again.");

        var client = await clientRepo.FirstOrDefaultAsync(new ExpressionSpecification<Client>(x => x.Id == currentUser.GetUserId()), cancellationToken);
        _ = client ?? throw new NotFoundException(localizer["client not found"]);

        if (request.SelectedOptionIds.Count > 0)
        {
            var selectedOptions = await customizationOptionRepo.ListAsync(new MealCustomizationOptionByIdsSpec(request.SelectedOptionIds), cancellationToken);

            entity.MealSelectionType = MealSelectionType.Custom;

            var customMeal = new CustomMeal()
            {
                BaseMealId = entity.MealId,
                ClientId = entity.ClientId,
                NameAr = request.CustomeMealName ?? entity.Meal?.NameAr,
                NameEn = request.CustomeMealName ?? entity.Meal?.NameAr,
                ImageUrl = selectedOptions.FirstOrDefault()?.ImageUrl,
                SelectedOptions = request.SelectedOptionIds.ConvertAll(id => new CustomMealOption { OptionId = id, Quantity = 1 }),
            };

            customMeal.TotalProtein = selectedOptions.Sum(x => x.ProteinAdjustment + (x.Group?.ProteinAdjustment).GetValueOrDefault(0));
            customMeal.TotalCalories = selectedOptions.Sum(x => x.CaloriesAdjustment + (x.Group?.CaloriesAdjustment).GetValueOrDefault(0));
            customMeal.TotalCarbs = selectedOptions.Sum(x => x.CarbsAdjustment + (x.Group?.CarbsAdjustment).GetValueOrDefault(0));
            customMeal.TotalFat = selectedOptions.Sum(x => x.FatAdjustment + (x.Group?.FatAdjustment).GetValueOrDefault(0));

            entity.CustomeMealName = request.CustomeMealName ?? entity.Meal?.NameEn;
            entity.CustomMealId = customMeal.Id;
            var newPrice = entity.BasePrice + selectedOptions.Sum(x => x.PriceAdjustment);
            customMeal.TotalPrice = newPrice;

            if (newPrice > entity.BasePrice && newPrice > entity.AdjustedPrice.GetValueOrDefault())
            {
                var cost = customMeal.TotalPrice - entity.BasePrice;
                entity.AdjustedPrice = newPrice;
                entity.PriceAdjustmentReason = $"Your meal was cusomize {customMeal.NameEn} (+{cost}).";

                if (string.IsNullOrEmpty(request.PaymentMethodId))
                {
                    logger.LogWarning("No payment method provided for daily meal selection {DailyMealSelectionId}", entity.Id);
                    throw new BadRequestException("To cusomize your meal, please add a payment method.");
                }

                await ProcessPaymentAsync(
                    entity,
                    client,
                    cost,
                    request.PaymentMethodId,
                    cancellationToken);
            }

            // Subtract old values
            dailySelection.TotalCarbohydrates -= entity.TotalCarbohydrates;
            dailySelection.TotalCalories -= entity.TotalCalories;
            dailySelection.TotalFats -= entity.TotalFats;
            dailySelection.TotalProteins -= entity.TotalProteins;

            // Add new values
            dailySelection.TotalCarbohydrates += customMeal.TotalCarbs;
            dailySelection.TotalCalories += customMeal.TotalCalories;
            dailySelection.TotalFats += customMeal.TotalFat;
            dailySelection.TotalProteins += customMeal.TotalProtein;

            entity.TotalCalories = customMeal.TotalCalories;
            entity.TotalProteins = customMeal.TotalProtein;
            entity.TotalFats = customMeal.TotalFat;
            entity.TotalCarbohydrates = customMeal.TotalCarbs;

            await customMealRepo.AddAsync(customMeal, cancellationToken);

            await repository.SaveChangesAsync(cancellationToken);

            return await Result<DefaultIdType>.SuccessAsync(data: customMeal.Id);

        }

        return await Result<DefaultIdType>.SuccessAsync(data: Guid.Empty);

    }

    private async Task<PaymentIntentResponseDTO> ProcessPaymentAsync(
        DailyMealSelection dailyMealSelection,
        Client? client,
        decimal cost,
        string paymentMethodId,
        CancellationToken cancellationToken)
    {
        var paymentIntent = await stripeService.CreateAddOnPaymentLink(
            dailyMealSelection.Id.ToString(),
            paymentMethodId,
            client!.StripeId!,
            cost,
            dailyMealSelection.PriceAdjustmentReason!);

        if (paymentIntent.Status != "succeeded")
        {
            logger.LogWarning("Payment failed for daily meal selection {DailyMealSelectionId} with status {Status}",
                dailyMealSelection.Id.ToString(), paymentIntent.Status);
            throw new BadRequestException(localizer["Payment processing failed"]);
        }

        var payment = CreatePaymentRecord(client, paymentIntent, dailyMealSelection.PriceAdjustmentReason);
        await paymentRepo.AddAsync(payment, withSaveChanges: false, cancellationToken: cancellationToken);
        logger.LogInformation("Created payment record {PaymentId}", payment.Id);

        await AddLoyaltyPoints(client, payment, dailyMealSelection.PriceAdjustmentReason, cancellationToken);
        dailyMealSelection.IsPaid = true;

        return paymentIntent;
    }

    private async Task AddLoyaltyPoints(
        Client client,
        Payment payment,
        string? mealDescription,
        CancellationToken cancellationToken)
    {
        logger.LogDebug("Calculating loyalty points for payment {PaymentId}", payment.Id);

        var points = await clientPortalSettingsService.GetOnePointEqualInMoney();
        var pointsEarned = points == 0 ? 0 : (int)(payment.Amount / points);

        await clientLoyaltyPointRepo.AddAsync(new ClientLoyaltyPoint
        {
            ClientId = client.Id,
            Reason = mealDescription,
            Source = "Payment",
            TransactionId = payment.Id.ToString(),
            Points = pointsEarned,
            Type = TransactionType.Earn,
            DateEarned = DateTime.UtcNow,
        }, withSaveChanges: false, cancellationToken: cancellationToken);

        logger.LogInformation("Awarded {PointsEarned} loyalty points to client {ClientId}",
            pointsEarned, client.Id);
    }

    private Payment CreatePaymentRecord(
        Client client,
        PaymentIntentResponseDTO paymentIntent,
        string? mealDescription)
    {
        logger.LogDebug("Creating payment record for payment intent {PaymentIntentId}", paymentIntent.Id);

        return new Payment
        {
            ClientId = client.Id,
            ClientSubscriptionId = client.ClientSubscriptionId!.Value,
            InvoiceNumber = paymentIntent.Id,
            TransactionId = paymentIntent.LatestChargeId ?? paymentIntent.Id,
            PaymentStatus = PaymentStatus.Paid,
            PaymentMethod = paymentIntent.PaymentMethodType ?? "unknown",
            Amount = paymentIntent.Amount / 100m,
            PaymentGateway = "Stripe",
            Reason = mealDescription,
            Currency = paymentIntent.Currency,
            PaymentDate = DateTime.UtcNow,
            OrderId = paymentIntent.OrderId
        };
    }
}

public class MealCustomizationOptionByIdsSpec : Specification<MealCustomizationOption>
{
    public MealCustomizationOptionByIdsSpec(List<DefaultIdType> ids)
    {
        Query.Include(x=>x.Group).Where(p => ids.Contains(p.Id));
    }
}
