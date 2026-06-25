using System.Text.Json;
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
using ZeroFat.NutriPlan.Domain.MealPlanning;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.Wizard;

public class GetWizardPreviewRequest(DefaultIdType draftId) : IQuery<Result<SubscriptionWizardPreviewDto>>
{
    public DefaultIdType DraftId { get; set; } = draftId;
}

public class GetWizardPreviewRequestHandler(
    ICurrentUser currentUser,
    IReadRepository<SubscriptionWizardDraft> draftRepo,
    IReadRepository<MealPlan> mealPlanRepo,
    IReadRepository<MealPlanMealType> mealPlanMealTypeRepo,
    IReadRepository<ClientLocation> locationRepo,
    ISubscriptionPricingService pricingService,
    IDeliveryCalendarService calendarService,
    IClientPortalSettingservice clientPortalSettingservice,
    IStringLocalizer<GetWizardPreviewRequestHandler> localizer) : IQueryHandler<GetWizardPreviewRequest, Result<SubscriptionWizardPreviewDto>>
{
    public async Task<Result<SubscriptionWizardPreviewDto>> Handle(GetWizardPreviewRequest request, CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);
        var draft = await SubscriptionWizardMapper.GetDraftOrThrowAsync(draftRepo, request.DraftId, localizer, cancellationToken);
        var (pricing, startDate) = await BuildPricingAsync(draft, mealPlanMealTypeRepo, pricingService, calendarService, clientPortalSettingservice, cancellationToken);

        var mealPlan = draft.MealPlanId.HasValue
            ? await mealPlanRepo.FirstOrDefaultAsync(new ExpressionSpecification<MealPlan>(x => x.Id == draft.MealPlanId), cancellationToken)
            : null;

        string? address = null;
        if (draft.ClientLocationId.HasValue)
        {
            var location = await locationRepo.FirstOrDefaultAsync(
                new ExpressionSpecification<ClientLocation>(x => x.Id == draft.ClientLocationId),
                cancellationToken);
            address = location?.FullAddressEn;
        }

        return await Result<SubscriptionWizardPreviewDto>.SuccessAsync(new SubscriptionWizardPreviewDto
        {
            PlanName = mealPlan?.NameEn,
            PlanDuration = draft.SubscriptionType?.ToString(),
            MealsPerDay = draft.MealTypeSelections.Sum(x => x.QuantityPerDay),
            DeliveryAddress = address,
            PlanAmount = pricing.PlanAmount,
            AddOnAmount = pricing.AddOnAmount,
            DiscountAmount = pricing.DiscountAmount,
            VatAmount = pricing.VatAmount,
            Total = pricing.TotalCost,
            AverageCalories = pricing.AverageCalories,
            StartDate = startDate,
            EndDate = pricing.EndDate
        });
    }

    internal static async Task<(SubscriptionPricingResult Pricing, DateOnly StartDate)> BuildPricingAsync(
        SubscriptionWizardDraft draft,
        IReadRepository<MealPlanMealType> mealPlanMealTypeRepo,
        ISubscriptionPricingService pricingService,
        IDeliveryCalendarService calendarService,
        IClientPortalSettingservice clientPortalSettingservice,
        CancellationToken cancellationToken)
    {
        if (draft.MealPlanId is null || draft.SubscriptionType is null)
        {
            throw new BadRequestException("Configuration and schedule must be completed before preview.");
        }

        var offset = await clientPortalSettingservice.GetOffsetSubscriptionInDays();
        var startDate = draft.ScheduledStartDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(offset));
        var deliveryDays = draft.SelectedDeliveryDays.Count > 0
            ? draft.SelectedDeliveryDays
            : calendarService.DeriveDeliveryDays(draft.SkipSaturdays, draft.SkipSundays);

        var mealSelections = new List<SubscriptionMealTypePricingInput>();
        foreach (var selection in draft.MealTypeSelections.Where(x => x.QuantityPerDay > 0))
        {
            var mealPlanMealType = await mealPlanMealTypeRepo.FirstOrDefaultAsync(
                new ExpressionSpecificationProjecting<MealPlanMealType, MealPlanMealTypeDto>(
                    x => x.MealPlanId == draft.MealPlanId && x.MealTypeId == selection.MealTypeId),
                cancellationToken);

            if (mealPlanMealType == null)
            {
                continue;
            }

            mealSelections.Add(new SubscriptionMealTypePricingInput
            {
                MealTypeId = selection.MealTypeId,
                QuantityPerDay = selection.QuantityPerDay,
                Price = mealPlanMealType.Price.GetValueOrDefault(),
                AverageCalories = mealPlanMealType.AverageCalories.GetValueOrDefault(),
                MealTypeNameEn = mealPlanMealType.MealType?.NameEn,
                MealTypeNameAr = mealPlanMealType.MealType?.NameAr
            });
        }

        var pricing = await pricingService.CalculateAsync(new SubscriptionPricingInput
        {
            SubscriptionType = draft.SubscriptionType.Value,
            SelectedDeliveryDays = deliveryDays,
            MealTypeSelections = mealSelections,
            AddOnItems = draft.AddOnItems,
            PromoCode = draft.PromoCode,
            ManualDiscountAed = draft.ManualDiscountAed,
            StartDate = startDate
        }, cancellationToken);

        return (pricing, startDate);
    }
}

public class GetWizardDeliveryCalendarRequest : IQuery<Result<DeliveryCalendarResult>>
{
    public DefaultIdType DraftId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
}

public class GetWizardDeliveryCalendarRequestHandler(
    ICurrentUser currentUser,
    IReadRepository<SubscriptionWizardDraft> draftRepo,
    IDeliveryCalendarService calendarService,
    IStringLocalizer<GetWizardDeliveryCalendarRequestHandler> localizer) : IQueryHandler<GetWizardDeliveryCalendarRequest, Result<DeliveryCalendarResult>>
{
    public async Task<Result<DeliveryCalendarResult>> Handle(GetWizardDeliveryCalendarRequest request, CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);
        var draft = await SubscriptionWizardMapper.GetDraftOrThrowAsync(draftRepo, request.DraftId, localizer, cancellationToken);

        if (draft.SubscriptionType is null)
        {
            throw new BadRequestException(localizer["Subscription duration is required."]);
        }

        var calendar = await calendarService.BuildCalendarAsync(new DeliveryCalendarInput
        {
            ClientId = draft.ClientId,
            SubscriptionType = draft.SubscriptionType.Value,
            SkipSaturdays = draft.SkipSaturdays,
            SkipSundays = draft.SkipSundays,
            RenewalStrategy = draft.RenewalStrategy,
            ScheduledStartDate = draft.ScheduledStartDate,
            Month = request.Month,
            Year = request.Year
        }, cancellationToken);

        return await Result<DeliveryCalendarResult>.SuccessAsync(calendar);
    }
}

public class GetWizardMealPlansRequest(DefaultIdType clientId) : IQuery<Result<List<WizardMealPlanOptionDto>>>
{
    public DefaultIdType ClientId { get; set; } = clientId;
}

public class GetWizardMealPlansRequestHandler(
    ICurrentUser currentUser,
    IReadRepository<MealPlan> mealPlanRepo,
    IStringLocalizer<GetWizardMealPlansRequestHandler> localizer) : IQueryHandler<GetWizardMealPlansRequest, Result<List<WizardMealPlanOptionDto>>>
{
    public async Task<Result<List<WizardMealPlanOptionDto>>> Handle(GetWizardMealPlansRequest request, CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);
        var plans = await mealPlanRepo.ListAsync(
            new ExpressionSpecification<MealPlan>(x => x.IsActive),
            cancellationToken);

        var result = plans.Select(plan => new WizardMealPlanOptionDto
        {
            Id = plan.Id,
            NameEn = plan.NameEn,
            NameAr = plan.NameAr,
            MealTypesCount = plan.MealPlanMealTypes.Count,
            AverageCalories = plan.MealPlanMealTypes.Sum(x => x.AverageCalories),
            MonthlyPrice = plan.MealPlanMealTypes.Sum(x => x.Price * 4)
        }).ToList();

        return await Result<List<WizardMealPlanOptionDto>>.SuccessAsync(result);
    }
}

public class GetWizardAddOnsRequest(SubscriptionAddOnCategory? category) : IQuery<Result<List<WizardAddOnOptionDto>>>
{
    public SubscriptionAddOnCategory? Category { get; set; } = category;
}

public class GetWizardAddOnsRequestHandler(
    ICurrentUser currentUser,
    IReadRepository<Meal> mealRepo,
    IStringLocalizer<GetWizardAddOnsRequestHandler> localizer) : IQueryHandler<GetWizardAddOnsRequest, Result<List<WizardAddOnOptionDto>>>
{
    public async Task<Result<List<WizardAddOnOptionDto>>> Handle(GetWizardAddOnsRequest request, CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);
        var meals = await mealRepo.ListAsync(new ExpressionSpecification<Meal>(x => x.IsAddOn && x.IsActive), cancellationToken);
        var result = meals.Select(meal => new WizardAddOnOptionDto
        {
            Id = meal.Id,
            NameEn = meal.NameEn,
            NameAr = meal.NameAr,
            Description = meal.NameEn,
            UnitPrice = (decimal)meal.PriceForCustomer,
            Category = request.Category ?? SubscriptionAddOnCategory.Meals
        }).ToList();

        return await Result<List<WizardAddOnOptionDto>>.SuccessAsync(result);
    }
}
