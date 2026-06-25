using System.Text.Json;
using Mapster;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.ClientSubscriptions;
using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.Wizard;

/// <summary>
/// Final wizard step: creates ClientSubscription (Pending), Stripe payment link, emails customer.
/// POST /api/clientPortal-module/ManageSubscriptions/wizard/{draftId}/finalize
/// </summary>
public class FinalizeSubscriptionWizardRequest(DefaultIdType draftId) : ICommand<Result<ClientSubscriptionSimplifyDto>>
{
    public DefaultIdType DraftId { get; set; } = draftId;
}

public class FinalizeSubscriptionWizardRequestHandler(
    ICurrentUser currentUser,
    IRepositoryWithEvents<SubscriptionWizardDraft> draftRepo,
    IRepositoryWithEvents<ClientSubscription> subscriptionRepo,
    IRepository<Client> clientRepo,
    IReadRepository<MealPlan> mealPlanRepo,
    IReadRepository<MealPlanMealType> mealPlanMealTypeRepo,
    ISubscriptionPricingService pricingService,
    IDeliveryCalendarService calendarService,
    IClientPortalSettingservice clientPortalSettingservice,
    IStripeService stripeService,
    IJobService jobService,
    IStringLocalizer<FinalizeSubscriptionWizardRequestHandler> localizer) : ICommandHandler<FinalizeSubscriptionWizardRequest, Result<ClientSubscriptionSimplifyDto>>
{
    public async Task<Result<ClientSubscriptionSimplifyDto>> Handle(FinalizeSubscriptionWizardRequest request, CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);

        var draft = await SubscriptionWizardMapper.GetDraftOrThrowAsync(draftRepo, request.DraftId, localizer, cancellationToken);
        if (string.IsNullOrWhiteSpace(draft.CustomerEmail))
        {
            throw new BadRequestException(localizer["Customer email is required."]);
        }

        var client = await clientRepo.FirstOrDefaultAsync(new ExpressionSpecification<Client>(x => x.Id == draft.ClientId), cancellationToken)
            ?? throw new NotFoundException(localizer["Client not found"]);

        if (draft.WizardMode == SubscriptionWizardMode.New &&
            client.ClientSubscriptionId != null &&
            client.SubscriptionStatus == SubscriptionStatus.Active)
        {
            throw new ForbiddenException(localizer["client already have subscription"]);
        }

        var mealPlan = await mealPlanRepo.FirstOrDefaultAsync(
            new ExpressionSpecification<MealPlan>(x => x.Id == draft.MealPlanId),
            cancellationToken) ?? throw new NotFoundException(localizer["Invalid Meal Plan."]);

        var (pricing, startDate) = await GetWizardPreviewRequestHandler.BuildPricingAsync(
            draft, mealPlanMealTypeRepo, pricingService, calendarService, clientPortalSettingservice, cancellationToken);

        var deliveryDays = draft.SelectedDeliveryDays.Count > 0
            ? draft.SelectedDeliveryDays
            : calendarService.DeriveDeliveryDays(draft.SkipSaturdays, draft.SkipSundays);

        var clientSubscription = new ClientSubscription
        {
            StartDate = startDate,
            EndDate = pricing.EndDate,
            SelectedDeliveryDays = deliveryDays,
            IsAutoRenewalEnabled = false,
            MealPlanId = draft.MealPlanId!.Value,
            PreferredDeliveryTime = draft.PreferredDeliveryTime ?? PreferredMealTime.Morning,
            ClientId = client.Id,
            PaymentStatus = PaymentStatus.Pending,
            SubscriptionStatus = SubscriptionStatus.Pending,
            SubscriptionType = draft.SubscriptionType!.Value,
            ClientLocationId = draft.ClientLocationId,
            PlanVariant = draft.PlanVariant,
            CalorieTarget = draft.CalorieTarget,
            ProteinTargetG = draft.ProteinTargetG,
            ManualDiscountAed = draft.ManualDiscountAed,
            PromoCode = draft.PromoCode,
            VatAmount = pricing.VatAmount,
            AddOnAmount = pricing.AddOnAmount,
            AddOnItems = draft.AddOnItems,
            CreatedByAdminId = currentUser.GetUserId(),
            TotalCost = pricing.TotalCost,
            AverageCalories = pricing.AverageCalories
        };

        foreach (var mealTypeSelection in draft.MealTypeSelections.Where(x => x.QuantityPerDay > 0))
        {
            var mealPlanMealType = await mealPlanMealTypeRepo.FirstOrDefaultAsync(
                new ExpressionSpecificationProjecting<MealPlanMealType, MealPlanMealTypeDto>(
                    x => x.MealPlanId == draft.MealPlanId && x.MealTypeId == mealTypeSelection.MealTypeId),
                cancellationToken)
                ?? throw new BadRequestException(localizer["error in subscription please contact our admin through chat."]);

            clientSubscription.SelectedMealTypes.Add(new MealTypeSelection
            {
                QuantityPerDay = mealTypeSelection.QuantityPerDay,
                MealTypeId = mealTypeSelection.MealTypeId,
                Price = mealPlanMealType.Price.GetValueOrDefault(),
                MealTypeNameAr = mealPlanMealType.MealType?.NameAr,
                MealTypeNameEn = mealPlanMealType.MealType?.NameEn
            });
        }

        var mealSelections = clientSubscription.SelectedMealTypes.Select(x => new
        {
            x.MealTypeId,
            MealTypeName = x.MealTypeNameEn,
            x.Price,
            x.QuantityPerDay
        }).ToList();

        var productName = ConstructProductDescription(clientSubscription.SelectedMealTypes, deliveryDays);
        var serializedMealSelections = JsonSerializer.Serialize(mealSelections);
        var offset = await clientPortalSettingservice.GetOffsetSubscriptionInDays();

        if (string.IsNullOrWhiteSpace(client.StripeId))
        {
            throw new BadRequestException(localizer["Client does not have a Stripe customer profile."]);
        }

        var (id, url) = await stripeService.CreateSubscriptionLink(
            clientSubscription.Id.ToString(),
            customerId: client.StripeId,
            productId: mealPlan.StripeId ?? string.Empty,
            amount: clientSubscription.TotalCost,
            mealSelections: serializedMealSelections,
            productName: mealPlan.NameEn!,
            description: productName,
            mealPlanId: mealPlan.Id,
            days: deliveryDays,
            isRecurring: false,
            subscriptionType: clientSubscription.SubscriptionType,
            startDate: clientSubscription.StartDate,
            endDate: clientSubscription.EndDate,
            offsetSubscription: offset,
            couponCode: draft.PromoCode);

        clientSubscription.PaymentQuickLink = url;
        clientSubscription.PaymentOrderId = id;
        clientSubscription.NextRenewalDate = clientSubscription.EndDate.AddDays(1 - offset);

        await subscriptionRepo.AddAsync(clientSubscription, withSaveChanges: true, cancellationToken: cancellationToken);
        client.ClientSubscriptionId = clientSubscription.Id;
        client.SubscriptionStatus = clientSubscription.SubscriptionStatus;
        await clientRepo.SaveChangesAsync(cancellationToken);

        draft.Status = SubscriptionDraftStatus.Finalized;
        draft.FinalizedClientSubscriptionId = clientSubscription.Id;
        await draftRepo.UpdateAsync(draft, cancellationToken);

        jobService.Enqueue<ISendSubscriptionPaymentLinkEmailJob>(x =>
            x.SendAsync(draft.CustomerEmail!, client.FullName ?? client.Email!, url!, draft.OptionalMessage, CancellationToken.None));

        return await Result<ClientSubscriptionSimplifyDto>.SuccessAsync(clientSubscription.Adapt<ClientSubscriptionSimplifyDto>());
    }

    private static string ConstructProductDescription(ICollection<MealTypeSelection> selections, List<DayOfWeek> days)
    {
        var productName = string.Join(", ", selections.Select(s => $"{s.QuantityPerDay} {s.MealTypeNameEn}"));
        productName += days.Count == 7 ? " (every day)" : $" (every {days.Count} days)";
        return productName;
    }
}
