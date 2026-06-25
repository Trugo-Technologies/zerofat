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
using ZeroFat.NutriPlan.Domain.MealPlanning;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.Wizard;

public class StartSubscriptionWizardRequest : ICommand<Result<SubscriptionWizardDraftDto>>
{
    public DefaultIdType ClientId { get; set; }
    public SubscriptionWizardMode WizardMode { get; set; }
}

public class StartSubscriptionWizardRequestHandler(
    ICurrentUser currentUser,
    IRepositoryWithEvents<SubscriptionWizardDraft> draftRepo,
    IReadRepository<Client> clientRepo,
    IReadRepository<ClientSubscription> subscriptionRepo,
    IReadRepository<MealPlan> mealPlanRepo,
    IStringLocalizer<StartSubscriptionWizardRequestHandler> localizer) : ICommandHandler<StartSubscriptionWizardRequest, Result<SubscriptionWizardDraftDto>>
{
    public async Task<Result<SubscriptionWizardDraftDto>> Handle(StartSubscriptionWizardRequest request, CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);

        var client = await clientRepo.FirstOrDefaultAsync(new ExpressionSpecification<Client>(x => x.Id == request.ClientId), cancellationToken)
            ?? throw new NotFoundException(localizer["Client not found"]);

        var draft = new SubscriptionWizardDraft
        {
            ClientId = request.ClientId,
            CreatedByAdminId = currentUser.GetUserId(),
            WizardMode = request.WizardMode,
            CurrentStep = request.WizardMode == SubscriptionWizardMode.Renew
                ? SubscriptionWizardStep.RenewalOptions
                : SubscriptionWizardStep.Configuration,
            Status = SubscriptionDraftStatus.Draft,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CustomerEmail = client.Email,
            SkipSaturdays = true,
            SkipSundays = true
        };

        if (request.WizardMode == SubscriptionWizardMode.Renew &&
            client.ClientSubscriptionId.HasValue &&
            client.SubscriptionStatus == SubscriptionStatus.Active)
        {
            var activeSub = await subscriptionRepo.FirstOrDefaultAsync(
                new ExpressionSpecification<ClientSubscription>(x => x.Id == client.ClientSubscriptionId),
                cancellationToken);

            if (activeSub != null)
            {
                draft.RenewalStrategy = SubscriptionRenewalStrategy.ExtendAfterExpiry;
                draft.MealPlanId = activeSub.MealPlanId;
                draft.SubscriptionType = activeSub.SubscriptionType;
                draft.SelectedDeliveryDays = activeSub.SelectedDeliveryDays;
                draft.PreferredDeliveryTime = activeSub.PreferredDeliveryTime;
                draft.ClientLocationId = activeSub.ClientLocationId;
                draft.MealTypeSelections = activeSub.SelectedMealTypes.Select(x => new WizardMealTypeSelection
                {
                    MealTypeId = x.MealTypeId,
                    Name = x.MealTypeNameEn,
                    QuantityPerDay = x.QuantityPerDay
                }).ToList();
            }
        }

        await draftRepo.AddAsync(draft, cancellationToken: cancellationToken);
        return await Result<SubscriptionWizardDraftDto>.SuccessAsync(
            await SubscriptionWizardMapper.MapAsync(draft, client, subscriptionRepo, mealPlanRepo, cancellationToken));
    }
}

public class GetSubscriptionWizardRequest(DefaultIdType draftId) : IQuery<Result<SubscriptionWizardDraftDto>>
{
    public DefaultIdType DraftId { get; set; } = draftId;
}

public class GetSubscriptionWizardRequestHandler(
    ICurrentUser currentUser,
    IReadRepository<SubscriptionWizardDraft> draftRepo,
    IReadRepository<Client> clientRepo,
    IReadRepository<ClientSubscription> subscriptionRepo,
    IReadRepository<MealPlan> mealPlanRepo,
    IStringLocalizer<GetSubscriptionWizardRequestHandler> localizer) : IQueryHandler<GetSubscriptionWizardRequest, Result<SubscriptionWizardDraftDto>>
{
    public async Task<Result<SubscriptionWizardDraftDto>> Handle(GetSubscriptionWizardRequest request, CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);
        var draft = await SubscriptionWizardMapper.GetDraftOrThrowAsync(draftRepo, request.DraftId, localizer, cancellationToken);
        var client = await clientRepo.FirstOrDefaultAsync(new ExpressionSpecification<Client>(x => x.Id == draft.ClientId), cancellationToken)
            ?? throw new NotFoundException(localizer["Client not found"]);
        return await Result<SubscriptionWizardDraftDto>.SuccessAsync(
            await SubscriptionWizardMapper.MapAsync(draft, client, subscriptionRepo, mealPlanRepo, cancellationToken));
    }
}

public class UpdateRenewalOptionsRequest : ICommand<Result<SubscriptionWizardDraftDto>>
{
    public DefaultIdType DraftId { get; set; }
    public SubscriptionRenewalStrategy RenewalStrategy { get; set; }
    public DateOnly? ScheduledStartDate { get; set; }
}

public class UpdateRenewalOptionsRequestHandler(
    ICurrentUser currentUser,
    IRepositoryWithEvents<SubscriptionWizardDraft> draftRepo,
    IReadRepository<Client> clientRepo,
    IReadRepository<ClientSubscription> subscriptionRepo,
    IReadRepository<MealPlan> mealPlanRepo,
    IStringLocalizer<UpdateRenewalOptionsRequestHandler> localizer) : ICommandHandler<UpdateRenewalOptionsRequest, Result<SubscriptionWizardDraftDto>>
{
    public async Task<Result<SubscriptionWizardDraftDto>> Handle(UpdateRenewalOptionsRequest request, CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);
        var draft = await SubscriptionWizardMapper.GetDraftOrThrowAsync(draftRepo, request.DraftId, localizer, cancellationToken);
        draft.RenewalStrategy = request.RenewalStrategy;
        draft.ScheduledStartDate = request.ScheduledStartDate;
        draft.CurrentStep = SubscriptionWizardStep.Configuration;
        await draftRepo.UpdateAsync(draft, cancellationToken);
        var client = await clientRepo.FirstOrDefaultAsync(new ExpressionSpecification<Client>(x => x.Id == draft.ClientId), cancellationToken)!;
        return await Result<SubscriptionWizardDraftDto>.SuccessAsync(
            await SubscriptionWizardMapper.MapAsync(draft, client!, subscriptionRepo, mealPlanRepo, cancellationToken));
    }
}

public class UpdateWizardConfigurationRequest : ICommand<Result<SubscriptionWizardDraftDto>>
{
    public DefaultIdType DraftId { get; set; }
    public DefaultIdType MealPlanId { get; set; }
    public string? PlanVariant { get; set; }
    public int? CalorieTarget { get; set; }
    public int? ProteinTargetG { get; set; }
    public List<WizardMealTypeSelection> MealTypeSelections { get; set; } = [];
    public List<SubscriptionWizardAddOnItem> AddOnItems { get; set; } = [];
}

public class UpdateWizardConfigurationRequestHandler(
    ICurrentUser currentUser,
    IRepositoryWithEvents<SubscriptionWizardDraft> draftRepo,
    IReadRepository<Client> clientRepo,
    IReadRepository<ClientSubscription> subscriptionRepo,
    IReadRepository<MealPlan> mealPlanRepo,
    IStringLocalizer<UpdateWizardConfigurationRequestHandler> localizer) : ICommandHandler<UpdateWizardConfigurationRequest, Result<SubscriptionWizardDraftDto>>
{
    public async Task<Result<SubscriptionWizardDraftDto>> Handle(UpdateWizardConfigurationRequest request, CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);
        var draft = await SubscriptionWizardMapper.GetDraftOrThrowAsync(draftRepo, request.DraftId, localizer, cancellationToken);
        draft.MealPlanId = request.MealPlanId;
        draft.PlanVariant = request.PlanVariant;
        draft.CalorieTarget = request.CalorieTarget;
        draft.ProteinTargetG = request.ProteinTargetG;
        draft.MealTypeSelections = request.MealTypeSelections;
        draft.AddOnItems = request.AddOnItems;
        draft.CurrentStep = SubscriptionWizardStep.Schedule;
        await draftRepo.UpdateAsync(draft, cancellationToken);
        var client = await clientRepo.FirstOrDefaultAsync(new ExpressionSpecification<Client>(x => x.Id == draft.ClientId), cancellationToken)!;
        return await Result<SubscriptionWizardDraftDto>.SuccessAsync(
            await SubscriptionWizardMapper.MapAsync(draft, client!, subscriptionRepo, mealPlanRepo, cancellationToken));
    }
}

public class UpdateWizardScheduleRequest : ICommand<Result<SubscriptionWizardDraftDto>>
{
    public DefaultIdType DraftId { get; set; }
    public SubscriptionType SubscriptionType { get; set; }
    public bool SkipSaturdays { get; set; }
    public bool SkipSundays { get; set; }
    public List<DateOnly>? SelectedDeliveryDates { get; set; }
    public PreferredMealTime PreferredDeliveryTime { get; set; }
    public DefaultIdType? ClientLocationId { get; set; }
    public string? PromoCode { get; set; }
    public decimal ManualDiscountAed { get; set; }
    public DateOnly? ScheduledStartDate { get; set; }
}

public class UpdateWizardScheduleRequestHandler(
    ICurrentUser currentUser,
    IRepositoryWithEvents<SubscriptionWizardDraft> draftRepo,
    IReadRepository<Client> clientRepo,
    IReadRepository<ClientSubscription> subscriptionRepo,
    IReadRepository<MealPlan> mealPlanRepo,
    IDeliveryCalendarService calendarService,
    IStringLocalizer<UpdateWizardScheduleRequestHandler> localizer) : ICommandHandler<UpdateWizardScheduleRequest, Result<SubscriptionWizardDraftDto>>
{
    public async Task<Result<SubscriptionWizardDraftDto>> Handle(UpdateWizardScheduleRequest request, CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);
        var draft = await SubscriptionWizardMapper.GetDraftOrThrowAsync(draftRepo, request.DraftId, localizer, cancellationToken);
        draft.SubscriptionType = request.SubscriptionType;
        draft.SkipSaturdays = request.SkipSaturdays;
        draft.SkipSundays = request.SkipSundays;
        draft.PreferredDeliveryTime = request.PreferredDeliveryTime;
        draft.ClientLocationId = request.ClientLocationId;
        draft.PromoCode = request.PromoCode;
        draft.ManualDiscountAed = request.ManualDiscountAed;
        draft.ScheduledStartDate = request.ScheduledStartDate;
        draft.SelectedDeliveryDays = calendarService.DeriveDeliveryDays(request.SkipSaturdays, request.SkipSundays);

        if (request.SelectedDeliveryDates?.Count > 0)
        {
            draft.SelectedDeliveryDates = request.SelectedDeliveryDates;
        }

        draft.CurrentStep = SubscriptionWizardStep.BillingReview;
        await draftRepo.UpdateAsync(draft, cancellationToken);
        var client = await clientRepo.FirstOrDefaultAsync(new ExpressionSpecification<Client>(x => x.Id == draft.ClientId), cancellationToken)!;
        return await Result<SubscriptionWizardDraftDto>.SuccessAsync(
            await SubscriptionWizardMapper.MapAsync(draft, client!, subscriptionRepo, mealPlanRepo, cancellationToken));
    }
}

public class UpdateWizardBillingRequest : ICommand<Result<SubscriptionWizardDraftDto>>
{
    public DefaultIdType DraftId { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public string? OptionalMessage { get; set; }
}

public class UpdateWizardBillingRequestHandler(
    ICurrentUser currentUser,
    IRepositoryWithEvents<SubscriptionWizardDraft> draftRepo,
    IReadRepository<Client> clientRepo,
    IReadRepository<ClientSubscription> subscriptionRepo,
    IReadRepository<MealPlan> mealPlanRepo,
    IStringLocalizer<UpdateWizardBillingRequestHandler> localizer) : ICommandHandler<UpdateWizardBillingRequest, Result<SubscriptionWizardDraftDto>>
{
    public async Task<Result<SubscriptionWizardDraftDto>> Handle(UpdateWizardBillingRequest request, CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);
        var draft = await SubscriptionWizardMapper.GetDraftOrThrowAsync(draftRepo, request.DraftId, localizer, cancellationToken);
        draft.CustomerEmail = request.CustomerEmail;
        draft.OptionalMessage = request.OptionalMessage;
        draft.CurrentStep = SubscriptionWizardStep.BillingReview;
        await draftRepo.UpdateAsync(draft, cancellationToken);
        var client = await clientRepo.FirstOrDefaultAsync(new ExpressionSpecification<Client>(x => x.Id == draft.ClientId), cancellationToken)!;
        return await Result<SubscriptionWizardDraftDto>.SuccessAsync(
            await SubscriptionWizardMapper.MapAsync(draft, client!, subscriptionRepo, mealPlanRepo, cancellationToken));
    }
}

public class SaveSubscriptionWizardDraftRequest(DefaultIdType draftId) : ICommand<Result<SubscriptionWizardDraftDto>>
{
    public DefaultIdType DraftId { get; set; } = draftId;
}

public class SaveSubscriptionWizardDraftRequestHandler(
    ICurrentUser currentUser,
    IReadRepository<SubscriptionWizardDraft> draftRepo,
    IReadRepository<Client> clientRepo,
    IReadRepository<ClientSubscription> subscriptionRepo,
    IReadRepository<MealPlan> mealPlanRepo,
    IStringLocalizer<SaveSubscriptionWizardDraftRequestHandler> localizer) : ICommandHandler<SaveSubscriptionWizardDraftRequest, Result<SubscriptionWizardDraftDto>>
{
    public async Task<Result<SubscriptionWizardDraftDto>> Handle(SaveSubscriptionWizardDraftRequest request, CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);
        var draft = await SubscriptionWizardMapper.GetDraftOrThrowAsync(draftRepo, request.DraftId, localizer, cancellationToken);
        var client = await clientRepo.FirstOrDefaultAsync(new ExpressionSpecification<Client>(x => x.Id == draft.ClientId), cancellationToken)!;
        return await Result<SubscriptionWizardDraftDto>.SuccessAsync(
            await SubscriptionWizardMapper.MapAsync(draft, client!, subscriptionRepo, mealPlanRepo, cancellationToken));
    }
}

public class CancelSubscriptionWizardRequest(DefaultIdType draftId) : ICommand<Result>
{
    public DefaultIdType DraftId { get; set; } = draftId;
}

public class CancelSubscriptionWizardRequestHandler(
    ICurrentUser currentUser,
    IRepositoryWithEvents<SubscriptionWizardDraft> draftRepo,
    IStringLocalizer<CancelSubscriptionWizardRequestHandler> localizer) : ICommandHandler<CancelSubscriptionWizardRequest, Result>
{
    public async Task<Result> Handle(CancelSubscriptionWizardRequest request, CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);
        var draft = await SubscriptionWizardMapper.GetDraftOrThrowAsync(draftRepo, request.DraftId, localizer, cancellationToken);
        draft.Status = SubscriptionDraftStatus.Cancelled;
        await draftRepo.UpdateAsync(draft, cancellationToken);
        return (Result)await Result.SuccessAsync();
    }
}
