using Ardalis.Specification;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.Wizard;

internal static class SubscriptionWizardMapper
{
    public static async Task<SubscriptionWizardDraft> GetDraftOrThrowAsync(
        IReadRepository<SubscriptionWizardDraft> draftRepo,
        DefaultIdType draftId,
        IStringLocalizer localizer,
        CancellationToken cancellationToken) =>
        await GetDraftOrThrowCoreAsync(draftRepo, draftId, localizer, cancellationToken);

    public static async Task<SubscriptionWizardDraft> GetDraftOrThrowAsync(
        IRepositoryWithEvents<SubscriptionWizardDraft> draftRepo,
        DefaultIdType draftId,
        IStringLocalizer localizer,
        CancellationToken cancellationToken)
    {
        var draft = await draftRepo.FirstOrDefaultAsync(
            new ExpressionSpecification<SubscriptionWizardDraft>(x => x.Id == draftId && x.Status == SubscriptionDraftStatus.Draft),
            cancellationToken);

        return draft ?? throw new NotFoundException(localizer["Subscription wizard draft not found"]);
    }

    private static async Task<SubscriptionWizardDraft> GetDraftOrThrowCoreAsync(
        IReadRepositoryBase<SubscriptionWizardDraft> draftRepo,
        DefaultIdType draftId,
        IStringLocalizer localizer,
        CancellationToken cancellationToken)
    {
        var draft = await draftRepo.FirstOrDefaultAsync(
            new ExpressionSpecification<SubscriptionWizardDraft>(x => x.Id == draftId && x.Status == SubscriptionDraftStatus.Draft),
            cancellationToken);

        return draft ?? throw new NotFoundException(localizer["Subscription wizard draft not found"]);
    }

    public static async Task<SubscriptionWizardDraftDto> MapAsync(
        SubscriptionWizardDraft draft,
        Client client,
        IReadRepository<ClientSubscription> subscriptionRepo,
        IReadRepository<MealPlan> mealPlanRepo,
        CancellationToken cancellationToken)
    {
        var dto = new SubscriptionWizardDraftDto
        {
            Id = draft.Id,
            ClientId = draft.ClientId,
            WizardMode = draft.WizardMode,
            CurrentStep = draft.CurrentStep,
            RenewalStrategy = draft.RenewalStrategy,
            Status = draft.Status,
            MealPlanId = draft.MealPlanId,
            PlanVariant = draft.PlanVariant,
            CalorieTarget = draft.CalorieTarget,
            ProteinTargetG = draft.ProteinTargetG,
            MealTypeSelections = draft.MealTypeSelections,
            AddOnItems = draft.AddOnItems,
            SubscriptionType = draft.SubscriptionType,
            SkipSaturdays = draft.SkipSaturdays,
            SkipSundays = draft.SkipSundays,
            SelectedDeliveryDates = draft.SelectedDeliveryDates,
            SelectedDeliveryDays = draft.SelectedDeliveryDays,
            PreferredDeliveryTime = draft.PreferredDeliveryTime,
            ClientLocationId = draft.ClientLocationId,
            PromoCode = draft.PromoCode,
            ManualDiscountAed = draft.ManualDiscountAed,
            CustomerEmail = draft.CustomerEmail,
            OptionalMessage = draft.OptionalMessage,
            ScheduledStartDate = draft.ScheduledStartDate,
            HasActiveSubscription = client.SubscriptionStatus == SubscriptionStatus.Active && client.ClientSubscriptionId.HasValue
        };

        if (client.ClientSubscriptionId.HasValue && client.SubscriptionStatus == SubscriptionStatus.Active)
        {
            var activeSub = await subscriptionRepo.FirstOrDefaultAsync(
                new ExpressionSpecification<ClientSubscription>(x => x.Id == client.ClientSubscriptionId),
                cancellationToken);

            if (activeSub != null)
            {
                dto.ActiveSubscriptionEndDate = activeSub.EndDate;
                if (activeSub.MealPlanId != default)
                {
                    var mealPlan = await mealPlanRepo.FirstOrDefaultAsync(
                        new ExpressionSpecification<MealPlan>(x => x.Id == activeSub.MealPlanId),
                        cancellationToken);
                    dto.ActiveMealPlanName = mealPlan?.NameEn;
                }
            }
        }

        return dto;
    }
}
