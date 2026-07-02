using Ardalis.Specification;
using Mapster;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.ClientPortal.Application.ClientManagement.Clients;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.ClientSubscriptions;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.ClientSubscriptions;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.DailyMealSelections;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.DailySelections;
using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.MealPlanning;
using ZeroFat.NutriPlan.Domain.MenuPlanning;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.Wizard;

internal static class ClientDeliveryCalendarHelper
{
    public static ClientDeliveryCalendarDayStatus MapDayStatus(
        DailySelection? selection,
        DateOnly date,
        DateOnly? subscriptionStart,
        DateOnly? subscriptionEnd,
        HashSet<DateOnly> movedTargetDates)
    {
        if (subscriptionStart == null || subscriptionEnd == null || date < subscriptionStart || date > subscriptionEnd)
        {
            return ClientDeliveryCalendarDayStatus.Unavailable;
        }

        if (selection == null)
        {
            return ClientDeliveryCalendarDayStatus.Pending;
        }

        if (selection.DailySelectionStatus == DailySelectionStatus.Delivered)
        {
            return ClientDeliveryCalendarDayStatus.Delivered;
        }

        if (selection.DailySelectionStatus == DailySelectionStatus.Paused)
        {
            return selection.ReplacementDate.HasValue
                ? ClientDeliveryCalendarDayStatus.Moved
                : ClientDeliveryCalendarDayStatus.Skipped;
        }

        if (selection.DailySelectionStatus == DailySelectionStatus.Cancelled)
        {
            return ClientDeliveryCalendarDayStatus.Skipped;
        }

        if (movedTargetDates.Contains(date))
        {
            return ClientDeliveryCalendarDayStatus.Moved;
        }

        return ClientDeliveryCalendarDayStatus.Pending;
    }

    public static async Task<(Client Client, ClientSubscription Subscription)> GetActiveSubscriptionAsync(
        IReadRepository<Client> clientRepo,
        IReadRepository<ClientSubscription> subscriptionRepo,
        DefaultIdType clientId,
        IStringLocalizer localizer,
        CancellationToken cancellationToken)
    {
        var client = await clientRepo.FirstOrDefaultAsync(new ExpressionSpecification<Client>(x => x.Id == clientId), cancellationToken)
            ?? throw new NotFoundException(localizer["Client not found"]);

        if (!client.ClientSubscriptionId.HasValue)
        {
            throw new BadRequestException(localizer["Client has no active subscription."]);
        }

        var subscription = await subscriptionRepo.FirstOrDefaultAsync(
            new ClientSubscriptionByIdSpec(client.ClientSubscriptionId.Value),
            cancellationToken)
            ?? throw new NotFoundException(localizer["Subscription not found"]);

        return (client, subscription);
    }

    public static async Task<List<DailySelection>> GetMonthSelectionsAsync(
        IReadRepository<DailySelection> selectionRepo,
        DefaultIdType clientId,
        DefaultIdType subscriptionId,
        int month,
        int year,
        CancellationToken cancellationToken)
    {
        var monthStart = new DateOnly(year, month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        return await selectionRepo.ListAsync(
            new ExpressionSpecification<DailySelection>(x =>
                x.ClientId == clientId &&
                x.ClientSubscriptionId == subscriptionId &&
                x.Date >= monthStart &&
                x.Date <= monthEnd),
            cancellationToken);
    }

    public static HashSet<DateOnly> BuildMovedTargetDates(IEnumerable<DailySelection> selections)
        => selections
            .Where(x => x.ReplacementDate.HasValue)
            .Select(x => x.ReplacementDate!.Value)
            .ToHashSet();

    public static async Task<ClientDeliveryCutoffSettingsDto> BuildCutoffSettingsAsync(
        IClientPortalSettingservice settings,
        CancellationToken cancellationToken)
    {
        var offsetDays = await settings.GetOffsetSubscriptionInDays();
        var cutoffTime = await settings.GetCutoffTime();
        return new ClientDeliveryCutoffSettingsDto
        {
            EnableCutoffRestriction = offsetDays > 0,
            OffsetSubscriptionDays = offsetDays,
            CutoffTimeUtc = cutoffTime
        };
    }

    public static void EnsureCutoffAllowsChange(
        DateOnly targetDate,
        ClientDeliveryCutoffSettingsDto cutoff,
        IStringLocalizer localizer)
    {
        if (!cutoff.EnableCutoffRestriction)
        {
            return;
        }

        var cutoffDateTime = targetDate
            .AddDays(-cutoff.OffsetSubscriptionDays)
            .ToDateTime(cutoff.CutoffTimeUtc, DateTimeKind.Utc);

        if (DateTime.UtcNow > cutoffDateTime)
        {
            throw new BadRequestException(localizer["Changes are not allowed after the cutoff time."]);
        }
    }

    public static async Task<DailySelection> GetDailySelectionForDateAsync(
        IReadRepository<DailySelection> selectionRepo,
        DefaultIdType clientId,
        DefaultIdType subscriptionId,
        DateOnly date,
        IStringLocalizer localizer,
        CancellationToken cancellationToken)
    {
        return await selectionRepo.FirstOrDefaultAsync(
            new DailySelectionByDateSpec<DailySelection>(date, clientId),
            cancellationToken)
            ?? throw new NotFoundException(localizer["No delivery found for the selected date."]);
    }

    public static List<DailySelection> ResolveScopeSelections(
        List<DailySelection> allSelections,
        DeliveryAdjustmentScope scope,
        DateOnly anchorDate,
        List<DefaultIdType>? mealTypeIds = null)
    {
        return scope switch
        {
            DeliveryAdjustmentScope.ThisDayOnly => allSelections.Where(x => x.Date == anchorDate).ToList(),
            DeliveryAdjustmentScope.AllUpcomingDays => allSelections.Where(x => x.Date >= anchorDate).ToList(),
            DeliveryAdjustmentScope.EntireOrder => allSelections.ToList(),
            DeliveryAdjustmentScope.SpecificMealsOnly => allSelections.Where(x => x.Date == anchorDate).ToList(),
            _ => allSelections.Where(x => x.Date == anchorDate).ToList()
        };
    }

    public static async Task<ClientDeliveryDayDetailDto> BuildDayDetailAsync(
        DefaultIdType clientId,
        DateOnly date,
        ClientSubscription subscription,
        IReadRepository<DailySelection> selectionRepo,
        IReadRepository<DailyMealSelection> mealSelectionRepo,
        IReadRepository<MealType> mealTypeRepo,
        IReadRepository<MealPlan> mealPlanRepo,
        IReadRepository<Payment> paymentRepo,
        IReadRepository<Meal> mealRepo,
        IClientPortalSettingservice settings,
        ClientDeliveryPaymentResultDto? lastPayment,
        CancellationToken cancellationToken)
    {
        var monthSelections = await GetMonthSelectionsAsync(
            selectionRepo, clientId, subscription.Id, date.Month, date.Year, cancellationToken);
        var movedTargets = BuildMovedTargetDates(monthSelections);

        var selection = await selectionRepo.FirstOrDefaultAsync(
            new DailySelectionByDateSpec<DailySelection>(date, clientId),
            cancellationToken);

        var status = MapDayStatus(selection, date, subscription.StartDate, subscription.EndDate, movedTargets);

        var mealPlan = await mealPlanRepo.FirstOrDefaultAsync(
            new ExpressionSpecification<MealPlan>(x => x.Id == subscription.MealPlanId),
            cancellationToken);

        var meals = selection == null
            ? []
            : await mealSelectionRepo.ListAsync(
                new ExpressionSpecification<DailyMealSelection>(x => x.DailySelectionId == selection.Id),
                cancellationToken);

        var mealIds = meals.Where(x => x.MealId.HasValue).Select(x => x.MealId!.Value).Distinct().ToList();
        var mealLookup = mealIds.Count == 0
            ? new Dictionary<DefaultIdType, Meal>()
            : (await mealRepo.ListAsync(new ExpressionSpecification<Meal>(x => mealIds.Contains(x.Id)), cancellationToken))
                .ToDictionary(x => x.Id);

        var mealTypeIds = meals.Where(x => x.MealTypeId.HasValue).Select(x => x.MealTypeId!.Value).Distinct().ToList();
        var mealTypeLookup = mealTypeIds.Count == 0
            ? new Dictionary<DefaultIdType, MealType>()
            : (await mealTypeRepo.ListAsync(new ExpressionSpecification<MealType>(x => mealTypeIds.Contains(x.Id)), cancellationToken))
                .ToDictionary(x => x.Id);

        var latestPayment = await paymentRepo.FirstOrDefaultAsync(
            new ExpressionSpecification<Payment>(x => x.ClientSubscriptionId == subscription.Id),
            cancellationToken);

        return new ClientDeliveryDayDetailDto
        {
            Date = date,
            Status = status,
            DailySelectionId = selection?.Id,
            ReplacementDate = selection?.ReplacementDate,
            ActiveMealPlanName = mealPlan?.NameEn,
            AverageCalories = subscription.AverageCalories,
            DeliveryPaymentMethod = latestPayment?.PaymentMethod ?? "Online",
            ScheduleLabel = status == ClientDeliveryCalendarDayStatus.Skipped ? "Skipped" : "Scheduled",
            CutoffSettings = await BuildCutoffSettingsAsync(settings, cancellationToken),
            LastPayment = lastPayment,
            Meals = meals.Select(x =>
            {
                mealLookup.TryGetValue(x.MealId ?? Guid.Empty, out var meal);
                var unitPrice = x.AdjustedPrice ?? x.BasePrice;
                return new ClientDeliveryDayMealDto
                {
                    Id = x.Id,
                    MealTypeId = x.MealTypeId,
                    MealTypeName = x.MealTypeId.HasValue && mealTypeLookup.TryGetValue(x.MealTypeId.Value, out var mt)
                        ? mt.NameEn
                        : x.MealSelectionType == MealSelectionType.AddOn ? "Add-On" : null,
                    MealName = x.CustomeMealName ?? meal?.NameEn,
                    MealId = x.MealId,
                    MealSelectionType = x.MealSelectionType,
                    Qty = x.Qty,
                    BasePrice = x.BasePrice,
                    AdjustedPrice = x.AdjustedPrice,
                    LineTotal = unitPrice * x.Qty,
                    IsPaid = x.IsPaid
                };
            }).ToList()
        };
    }

    public static decimal CalculateMealUpgradeCost(DailyMealSelection entity, DailyMenuMeal dailyMenuMeal)
    {
        var menuPrice = dailyMenuMeal.DailyMenu?.Price;
        if (!menuPrice.HasValue)
        {
            return 0;
        }

        if (menuPrice.Value > entity.BasePrice && menuPrice.Value > entity.AdjustedPrice.GetValueOrDefault())
        {
            return menuPrice.Value - entity.BasePrice;
        }

        return 0;
    }
}

public class GetClientDeliveryCalendarRequest : IQuery<Result<ClientDeliveryCalendarResultDto>>
{
    public DefaultIdType ClientId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public ClientDeliveryCalendarDayStatus? Status { get; set; }
}

public class GetClientDeliveryCalendarRequestHandler(
    ICurrentUser currentUser,
    IReadRepository<Client> clientRepo,
    IReadRepository<ClientSubscription> subscriptionRepo,
    IReadRepository<DailySelection> selectionRepo,
    IReadRepository<DailyMealSelection> mealSelectionRepo,
    IStringLocalizer<GetClientDeliveryCalendarRequestHandler> localizer)
    : IQueryHandler<GetClientDeliveryCalendarRequest, Result<ClientDeliveryCalendarResultDto>>
{
    public async Task<Result<ClientDeliveryCalendarResultDto>> Handle(
        GetClientDeliveryCalendarRequest request,
        CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);
        var (_, subscription) = await ClientDeliveryCalendarHelper.GetActiveSubscriptionAsync(
            clientRepo, subscriptionRepo, request.ClientId, localizer, cancellationToken);

        var selections = await ClientDeliveryCalendarHelper.GetMonthSelectionsAsync(
            selectionRepo, request.ClientId, subscription.Id, request.Month, request.Year, cancellationToken);

        var movedTargets = ClientDeliveryCalendarHelper.BuildMovedTargetDates(selections);
        var selectionByDate = selections.ToDictionary(x => x.Date);
        var monthStart = new DateOnly(request.Year, request.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        var mealCounts = await mealSelectionRepo.ListAsync(
            new ExpressionSpecification<DailyMealSelection>(x =>
                x.ClientId == request.ClientId &&
                x.ClientSubscriptionId == subscription.Id &&
                x.Date >= monthStart &&
                x.Date <= monthEnd),
            cancellationToken);

        var mealCountByDate = mealCounts
            .GroupBy(x => x.Date)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Qty));

        var days = new List<ClientDeliveryCalendarDayDto>();
        for (var date = monthStart; date <= monthEnd; date = date.AddDays(1))
        {
            selectionByDate.TryGetValue(date, out var selection);
            var status = ClientDeliveryCalendarHelper.MapDayStatus(
                selection, date, subscription.StartDate, subscription.EndDate, movedTargets);

            if (request.Status.HasValue && request.Status.Value != status)
            {
                continue;
            }

            days.Add(new ClientDeliveryCalendarDayDto
            {
                Date = date,
                Status = status,
                MealCount = mealCountByDate.GetValueOrDefault(date),
                DailySelectionId = selection?.Id
            });
        }

        var result = new ClientDeliveryCalendarResultDto
        {
            Month = request.Month,
            Year = request.Year,
            Days = days,
            Summary = new ClientDeliveryCalendarSummaryDto
            {
                Delivered = days.Count(x => x.Status == ClientDeliveryCalendarDayStatus.Delivered),
                Pending = days.Count(x => x.Status == ClientDeliveryCalendarDayStatus.Pending),
                Skipped = days.Count(x => x.Status == ClientDeliveryCalendarDayStatus.Skipped),
                Moved = days.Count(x => x.Status == ClientDeliveryCalendarDayStatus.Moved)
            }
        };

        return await Result<ClientDeliveryCalendarResultDto>.SuccessAsync(result);
    }
}

public class GetClientDeliveryDayDetailRequest(DefaultIdType clientId, DateOnly date) : IQuery<Result<ClientDeliveryDayDetailDto>>
{
    public DefaultIdType ClientId { get; set; } = clientId;
    public DateOnly Date { get; set; } = date;
}

public class GetClientDeliveryDayDetailRequestHandler(
    ICurrentUser currentUser,
    IReadRepository<Client> clientRepo,
    IReadRepository<ClientSubscription> subscriptionRepo,
    IReadRepository<DailySelection> selectionRepo,
    IReadRepository<DailyMealSelection> mealSelectionRepo,
    IReadRepository<MealType> mealTypeRepo,
    IReadRepository<MealPlan> mealPlanRepo,
    IReadRepository<Payment> paymentRepo,
    IReadRepository<Meal> mealRepo,
    IClientPortalSettingservice settings,
    IStringLocalizer<GetClientDeliveryDayDetailRequestHandler> localizer)
    : IQueryHandler<GetClientDeliveryDayDetailRequest, Result<ClientDeliveryDayDetailDto>>
{
    public async Task<Result<ClientDeliveryDayDetailDto>> Handle(
        GetClientDeliveryDayDetailRequest request,
        CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);
        var (_, subscription) = await ClientDeliveryCalendarHelper.GetActiveSubscriptionAsync(
            clientRepo, subscriptionRepo, request.ClientId, localizer, cancellationToken);

        var dto = await ClientDeliveryCalendarHelper.BuildDayDetailAsync(
            request.ClientId, request.Date, subscription,
            selectionRepo, mealSelectionRepo, mealTypeRepo, mealPlanRepo, paymentRepo, mealRepo, settings, null, cancellationToken);

        return await Result<ClientDeliveryDayDetailDto>.SuccessAsync(dto);
    }
}

public class ChangeClientDeliveryMethodRequest : ICommand<Result<ClientDeliveryDayDetailDto>>
{
    public DefaultIdType ClientId { get; set; }
    public DateOnly Date { get; set; }
    public string PaymentMethod { get; set; } = "Online";
}

public class ChangeClientDeliveryMethodRequestHandler(
    ICurrentUser currentUser,
    IReadRepository<Client> clientRepo,
    IReadRepository<ClientSubscription> subscriptionRepo,
    IReadRepository<DailySelection> selectionRepo,
    IReadRepository<DailyMealSelection> mealSelectionRepo,
    IReadRepository<MealType> mealTypeRepo,
    IReadRepository<MealPlan> mealPlanRepo,
    IReadRepository<Payment> paymentRepo,
    IReadRepository<Meal> mealRepo,
    IRepository<Payment> paymentWriteRepo,
    IRepository<ClientAccountActivityLog> activityLogRepo,
    IClientPortalSettingservice settings,
    IStringLocalizer<ChangeClientDeliveryMethodRequestHandler> localizer)
    : ICommandHandler<ChangeClientDeliveryMethodRequest, Result<ClientDeliveryDayDetailDto>>
{
    public async Task<Result<ClientDeliveryDayDetailDto>> Handle(
        ChangeClientDeliveryMethodRequest request,
        CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);
        var cutoff = await ClientDeliveryCalendarHelper.BuildCutoffSettingsAsync(settings, cancellationToken);
        ClientDeliveryCalendarHelper.EnsureCutoffAllowsChange(request.Date, cutoff, localizer);

        var (_, subscription) = await ClientDeliveryCalendarHelper.GetActiveSubscriptionAsync(
            clientRepo, subscriptionRepo, request.ClientId, localizer, cancellationToken);

        var payment = await paymentWriteRepo.FirstOrDefaultAsync(
            new ExpressionSpecification<Payment>(x => x.ClientSubscriptionId == subscription.Id),
            cancellationToken);

        var previousMethod = payment?.PaymentMethod ?? "Online";

        if (payment != null)
        {
            payment.PaymentMethod = request.PaymentMethod;
            await paymentWriteRepo.UpdateAsync(payment, cancellationToken);
        }

        await ClientAccountActivityLogHelper.LogAsync(
            activityLogRepo, currentUser, request.ClientId,
            ClientAccountActivityAction.DeliveryMethodChanged,
            previousMethod,
            request.PaymentMethod,
            request.Date,
            cancellationToken: cancellationToken);

        var dto = await ClientDeliveryCalendarHelper.BuildDayDetailAsync(
            request.ClientId, request.Date, subscription,
            selectionRepo, mealSelectionRepo, mealTypeRepo, mealPlanRepo, paymentRepo, mealRepo, settings, null, cancellationToken);

        return await Result<ClientDeliveryDayDetailDto>.SuccessAsync(dto);
    }
}

public class CancelClientDeliveryRequest : ICommand<Result<ClientDeliveryDayDetailDto>>
{
    public DefaultIdType ClientId { get; set; }
    public DateOnly Date { get; set; }
    public DeliveryAdjustmentScope Scope { get; set; }
    public List<DefaultIdType> MealTypeIds { get; set; } = [];
}

public class CancelClientDeliveryRequestHandler(
    ICurrentUser currentUser,
    IReadRepository<Client> clientRepo,
    IReadRepository<ClientSubscription> subscriptionRepo,
    IReadRepository<DailySelection> selectionReadRepo,
    IRepository<DailySelection> selectionRepo,
    IRepository<DailyMealSelection> mealSelectionRepo,
    IReadRepository<DailyMealSelection> mealSelectionReadRepo,
    IReadRepository<MealType> mealTypeRepo,
    IReadRepository<MealPlan> mealPlanRepo,
    IReadRepository<Payment> paymentRepo,
    IReadRepository<Meal> mealRepo,
    IRepository<ClientAccountActivityLog> activityLogRepo,
    IClientPortalSettingservice settings,
    IStringLocalizer<CancelClientDeliveryRequestHandler> localizer)
    : ICommandHandler<CancelClientDeliveryRequest, Result<ClientDeliveryDayDetailDto>>
{
    public async Task<Result<ClientDeliveryDayDetailDto>> Handle(
        CancelClientDeliveryRequest request,
        CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);
        var cutoff = await ClientDeliveryCalendarHelper.BuildCutoffSettingsAsync(settings, cancellationToken);
        ClientDeliveryCalendarHelper.EnsureCutoffAllowsChange(request.Date, cutoff, localizer);

        var (_, subscription) = await ClientDeliveryCalendarHelper.GetActiveSubscriptionAsync(
            clientRepo, subscriptionRepo, request.ClientId, localizer, cancellationToken);

        var allSelections = await selectionReadRepo.ListAsync(
            new ExpressionSpecification<DailySelection>(x =>
                x.ClientId == request.ClientId && x.ClientSubscriptionId == subscription.Id),
            cancellationToken);

        var targets = ClientDeliveryCalendarHelper.ResolveScopeSelections(allSelections, request.Scope, request.Date);

        foreach (var selection in targets)
        {
            if (request.Scope == DeliveryAdjustmentScope.SpecificMealsOnly && request.MealTypeIds.Count > 0)
            {
                var meals = await mealSelectionReadRepo.ListAsync(
                    new ExpressionSpecification<DailyMealSelection>(x =>
                        x.DailySelectionId == selection.Id &&
                        x.MealTypeId.HasValue &&
                        request.MealTypeIds.Contains(x.MealTypeId.Value)),
                    cancellationToken);

                foreach (var meal in meals)
                {
                    selection.TotalCalories -= meal.TotalCalories;
                    selection.TotalProteins -= meal.TotalProteins;
                    selection.TotalFats -= meal.TotalFats;
                    selection.TotalCarbohydrates -= meal.TotalCarbohydrates;
                    await mealSelectionRepo.DeleteAsync(meal, cancellationToken);
                }

                await selectionRepo.UpdateAsync(selection, cancellationToken);
                continue;
            }

            selection.DailySelectionStatus = DailySelectionStatus.Paused;
            selection.ReplacementDate = null;
            await selectionRepo.UpdateAsync(selection, cancellationToken);
        }

        var cancelledLabel = request.Scope == DeliveryAdjustmentScope.SpecificMealsOnly
            ? "Specific meals cancelled"
            : "Cancelled";

        await ClientAccountActivityLogHelper.LogAsync(
            activityLogRepo, currentUser, request.ClientId,
            ClientAccountActivityAction.DeliveryCancelled,
            request.Date.ToString("yyyy-MM-dd"),
            cancelledLabel,
            request.Date,
            cancellationToken: cancellationToken);

        var dto = await ClientDeliveryCalendarHelper.BuildDayDetailAsync(
            request.ClientId, request.Date, subscription,
            selectionReadRepo, mealSelectionReadRepo, mealTypeRepo, mealPlanRepo, paymentRepo, mealRepo, settings, null, cancellationToken);

        return await Result<ClientDeliveryDayDetailDto>.SuccessAsync(dto);
    }
}

public class MoveClientDeliveryRequest : ICommand<Result<ClientDeliveryDayDetailDto>>
{
    public DefaultIdType ClientId { get; set; }
    public DateOnly Date { get; set; }
    public DateOnly NewDeliveryDate { get; set; }
    public DeliveryAdjustmentScope Scope { get; set; }
    public List<DefaultIdType> MealTypeIds { get; set; } = [];
}

public class MoveClientDeliveryRequestHandler(
    ICurrentUser currentUser,
    IReadRepository<Client> clientRepo,
    IReadRepository<ClientSubscription> subscriptionRepo,
    IReadRepository<DailySelection> selectionReadRepo,
    IRepository<DailySelection> selectionRepo,
    IReadRepository<DailyMealSelection> mealSelectionReadRepo,
    IReadRepository<MealType> mealTypeRepo,
    IReadRepository<MealPlan> mealPlanRepo,
    IReadRepository<Payment> paymentRepo,
    IReadRepository<Meal> mealRepo,
    IRepository<ClientAccountActivityLog> activityLogRepo,
    IClientPortalSettingservice settings,
    IStringLocalizer<MoveClientDeliveryRequestHandler> localizer)
    : ICommandHandler<MoveClientDeliveryRequest, Result<ClientDeliveryDayDetailDto>>
{
    public async Task<Result<ClientDeliveryDayDetailDto>> Handle(
        MoveClientDeliveryRequest request,
        CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);
        var cutoff = await ClientDeliveryCalendarHelper.BuildCutoffSettingsAsync(settings, cancellationToken);
        ClientDeliveryCalendarHelper.EnsureCutoffAllowsChange(request.Date, cutoff, localizer);

        var (_, subscription) = await ClientDeliveryCalendarHelper.GetActiveSubscriptionAsync(
            clientRepo, subscriptionRepo, request.ClientId, localizer, cancellationToken);

        var allSelections = await selectionReadRepo.ListAsync(
            new ExpressionSpecification<DailySelection>(x =>
                x.ClientId == request.ClientId && x.ClientSubscriptionId == subscription.Id),
            cancellationToken);

        var targets = ClientDeliveryCalendarHelper.ResolveScopeSelections(allSelections, request.Scope, request.Date);

        foreach (var selection in targets)
        {
            selection.DailySelectionStatus = DailySelectionStatus.Paused;
            selection.ReplacementDate = request.NewDeliveryDate;
            await selectionRepo.UpdateAsync(selection, cancellationToken);
        }

        var destination = await selectionReadRepo.FirstOrDefaultAsync(
            new DailySelectionByDateSpec<DailySelection>(request.NewDeliveryDate, request.ClientId),
            cancellationToken);

        if (destination != null && destination.DailySelectionStatus == DailySelectionStatus.Paused)
        {
            destination.DailySelectionStatus = DailySelectionStatus.Pending;
            destination.ReplacementDate = null;
            await selectionRepo.UpdateAsync(destination, cancellationToken);
        }

        await ClientAccountActivityLogHelper.LogAsync(
            activityLogRepo, currentUser, request.ClientId,
            ClientAccountActivityAction.DeliveryMoved,
            request.Date.ToString("yyyy-MM-dd"),
            request.NewDeliveryDate.ToString("yyyy-MM-dd"),
            request.Date,
            cancellationToken: cancellationToken);

        var dto = await ClientDeliveryCalendarHelper.BuildDayDetailAsync(
            request.ClientId, request.Date, subscription,
            selectionReadRepo, mealSelectionReadRepo, mealTypeRepo, mealPlanRepo, paymentRepo, mealRepo, settings, null, cancellationToken);

        return await Result<ClientDeliveryDayDetailDto>.SuccessAsync(dto);
    }
}

public class AddClientDeliveryItemsRequest : ICommand<Result<ClientDeliveryDayDetailDto>>
{
    public DefaultIdType ClientId { get; set; }
    public DateOnly Date { get; set; }
    public List<ClientDeliveryAddOnItemDto> Items { get; set; } = [];
    public string? PaymentMethodId { get; set; }
    public bool WaivePayment { get; set; }
}

public class AddClientDeliveryItemsRequestHandler(
    ICurrentUser currentUser,
    IReadRepository<Client> clientRepo,
    IReadRepository<ClientSubscription> subscriptionRepo,
    IReadRepository<DailySelection> selectionReadRepo,
    IRepository<DailySelection> selectionRepo,
    IRepository<DailyMealSelection> mealSelectionRepo,
    IReadRepository<DailyMealSelection> mealSelectionReadRepo,
    IReadRepository<Meal> mealRepo,
    IReadRepository<MealType> mealTypeRepo,
    IReadRepository<MealPlan> mealPlanRepo,
    IReadRepository<Payment> paymentRepo,
    IRepository<Payment> paymentWriteRepo,
    IReadRepository<ClientPaymentMethod> clientPaymentMethodRepo,
    IRepository<ClientLoyaltyPoint> loyaltyRepo,
    IRepository<ClientAccountActivityLog> activityLogRepo,
    IStripeService stripeService,
    IClientPortalSettingservice settings,
    IStringLocalizer<AddClientDeliveryItemsRequestHandler> localizer)
    : ICommandHandler<AddClientDeliveryItemsRequest, Result<ClientDeliveryDayDetailDto>>
{
    public async Task<Result<ClientDeliveryDayDetailDto>> Handle(
        AddClientDeliveryItemsRequest request,
        CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);
        var cutoff = await ClientDeliveryCalendarHelper.BuildCutoffSettingsAsync(settings, cancellationToken);
        ClientDeliveryCalendarHelper.EnsureCutoffAllowsChange(request.Date, cutoff, localizer);

        var (client, subscription) = await ClientDeliveryCalendarHelper.GetActiveSubscriptionAsync(
            clientRepo, subscriptionRepo, request.ClientId, localizer, cancellationToken);

        var selection = await ClientDeliveryCalendarHelper.GetDailySelectionForDateAsync(
            selectionReadRepo, request.ClientId, subscription.Id, request.Date, localizer, cancellationToken);

        decimal totalCost = 0;
        var mealDescriptions = new List<string>();
        var pendingSelections = new List<DailyMealSelection>();

        foreach (var item in request.Items)
        {
            var meal = await mealRepo.GetByIdAsync(item.MealId, cancellationToken)
                ?? throw new NotFoundException(localizer["Meal not found"]);

            if (!meal.IsAddOn || meal.PriceForCustomer <= 0)
            {
                throw new BadRequestException(localizer["Only add-on meals can be added."]);
            }

            var lineTotal = (decimal)meal.PriceForCustomer * item.Quantity;
            totalCost += lineTotal;
            mealDescriptions.Add($"{item.Quantity}x {meal.NameEn}");

            pendingSelections.Add(new DailyMealSelection
            {
                ClientId = request.ClientId,
                ClientSubscriptionId = subscription.Id,
                DailySelectionId = selection.Id,
                Date = selection.Date,
                MealId = meal.Id,
                MealPlanId = selection.MealPlanId,
                MealSelectionType = MealSelectionType.AddOn,
                Qty = item.Quantity,
                BasePrice = (decimal)meal.PriceForCustomer,
                TotalCalories = meal.Calories * item.Quantity,
                TotalProteins = meal.Protein * item.Quantity,
                TotalFats = meal.Fat * item.Quantity,
                TotalCarbohydrates = meal.Carbs * item.Quantity
            });
        }

        var paymentMethodId = await ClientDeliveryCalendarPaymentHelper.ResolvePaymentMethodIdAsync(
            request.ClientId, request.PaymentMethodId, request.WaivePayment, totalCost,
            clientPaymentMethodRepo, localizer, cancellationToken);

        var description = $"Additional items for {request.Date:yyyy-MM-dd}: {string.Join(", ", mealDescriptions)}";
        var paymentResult = await ClientDeliveryCalendarPaymentHelper.ChargeAsync(
            client, subscription, totalCost, description, selection.Id.ToString(),
            paymentMethodId, request.WaivePayment, stripeService, paymentWriteRepo, settings, loyaltyRepo,
            localizer, cancellationToken);

        foreach (var mealSelection in pendingSelections)
        {
            selection.TotalCalories += mealSelection.TotalCalories;
            selection.TotalProteins += mealSelection.TotalProteins;
            selection.TotalFats += mealSelection.TotalFats;
            selection.TotalCarbohydrates += mealSelection.TotalCarbohydrates;
            mealSelection.IsPaid = paymentResult.AmountCharged > 0 || request.WaivePayment;
            await mealSelectionRepo.AddAsync(mealSelection, cancellationToken);
        }

        await selectionRepo.UpdateAsync(selection, cancellationToken);

        await ClientAccountActivityLogHelper.LogAsync(
            activityLogRepo, currentUser, request.ClientId,
            ClientAccountActivityAction.AdditionalItemAdded,
            "—",
            string.Join(", ", mealDescriptions),
            request.Date,
            cancellationToken: cancellationToken);

        var dto = await ClientDeliveryCalendarHelper.BuildDayDetailAsync(
            request.ClientId, request.Date, subscription,
            selectionReadRepo, mealSelectionReadRepo, mealTypeRepo, mealPlanRepo, paymentRepo, mealRepo, settings,
            paymentResult, cancellationToken);

        return await Result<ClientDeliveryDayDetailDto>.SuccessAsync(dto);
    }
}

public class ChangeClientDeliveryMealPlanRequest : ICommand<Result<ClientDeliveryDayDetailDto>>
{
    public DefaultIdType ClientId { get; set; }
    public DateOnly Date { get; set; }
    public DefaultIdType? MealPlanId { get; set; }
    public string? PlanVariant { get; set; }
    public int? CalorieTarget { get; set; }
    public int? MealsPerDay { get; set; }
    public List<ClientDeliveryMealPlanSlotDto> MealSlots { get; set; } = [];
    public bool ApplyStartingFromThisDay { get; set; }
    public string? PaymentMethodId { get; set; }
    public bool WaivePayment { get; set; }
}

public class ChangeClientDeliveryMealPlanRequestHandler(
    ICurrentUser currentUser,
    IReadRepository<Client> clientRepo,
    IRepository<ClientSubscription> subscriptionRepo,
    IReadRepository<ClientSubscription> subscriptionReadRepo,
    IReadRepository<DailySelection> selectionReadRepo,
    IRepository<DailySelection> selectionWriteRepo,
    IReadRepository<DailyMealSelection> mealSelectionReadRepo,
    IReadRepository<MealType> mealTypeRepo,
    IReadRepository<MealPlan> mealPlanRepo,
    IReadRepository<MealPlanMealType> mealPlanMealTypeRepo,
    IReadRepository<Payment> paymentRepo,
    IRepository<Payment> paymentWriteRepo,
    IReadRepository<ClientPaymentMethod> clientPaymentMethodRepo,
    IRepository<ClientLoyaltyPoint> loyaltyRepo,
    IRepository<ClientAccountActivityLog> activityLogRepo,
    IReadRepository<Meal> mealRepo,
    ISubscriptionPricingService pricingService,
    IStripeService stripeService,
    IClientPortalSettingservice settings,
    IStringLocalizer<ChangeClientDeliveryMealPlanRequestHandler> localizer)
    : ICommandHandler<ChangeClientDeliveryMealPlanRequest, Result<ClientDeliveryDayDetailDto>>
{
    public async Task<Result<ClientDeliveryDayDetailDto>> Handle(
        ChangeClientDeliveryMealPlanRequest request,
        CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);
        var cutoff = await ClientDeliveryCalendarHelper.BuildCutoffSettingsAsync(settings, cancellationToken);
        ClientDeliveryCalendarHelper.EnsureCutoffAllowsChange(request.Date, cutoff, localizer);

        var (client, subscription) = await ClientDeliveryCalendarHelper.GetActiveSubscriptionAsync(
            clientRepo, subscriptionReadRepo, request.ClientId, localizer, cancellationToken);

        var oldMealPlan = await mealPlanRepo.GetByIdAsync(subscription.MealPlanId, cancellationToken);
        var previousPlanLabel =
            $"{subscription.CalorieTarget ?? (int)subscription.AverageCalories} kcal {subscription.PlanVariant ?? oldMealPlan?.NameEn}";

        var oldPricing = await pricingService.CalculateAsync(
            ClientDeliveryCalendarPricingHelper.BuildPricingInput(subscription), cancellationToken);

        var targetMealPlanId = request.MealPlanId ?? subscription.MealPlanId;
        var mealSlots = request.MealSlots.Count > 0
            ? request.MealSlots
            : subscription.SelectedMealTypes.Select(x => new ClientDeliveryMealPlanSlotDto
            {
                MealTypeId = x.MealTypeId,
                Enabled = true,
                QuantityPerDay = x.QuantityPerDay
            }).ToList();

        var newMealTypeInputs = await ClientDeliveryCalendarPricingHelper.BuildMealTypePricingInputsAsync(
            targetMealPlanId, mealSlots, mealPlanMealTypeRepo, cancellationToken);

        var newPricingInput = ClientDeliveryCalendarPricingHelper.BuildPricingInput(subscription);
        newPricingInput.MealTypeSelections = newMealTypeInputs;
        var newPricing = await pricingService.CalculateAsync(newPricingInput, cancellationToken);

        var chargeAmount = ClientDeliveryCalendarPricingHelper.CalculateProratedUpgradeAmount(
            subscription, oldPricing.TotalCost, newPricing.TotalCost);

        var paymentMethodId = await ClientDeliveryCalendarPaymentHelper.ResolvePaymentMethodIdAsync(
            request.ClientId, request.PaymentMethodId, request.WaivePayment, chargeAmount,
            clientPaymentMethodRepo, localizer, cancellationToken);

        var paymentResult = await ClientDeliveryCalendarPaymentHelper.ChargeAsync(
            client, subscription, chargeAmount,
            $"Meal plan change for {request.Date:yyyy-MM-dd}",
            subscription.Id.ToString(),
            paymentMethodId, request.WaivePayment, stripeService, paymentWriteRepo, settings, loyaltyRepo,
            localizer, cancellationToken);

        if (request.MealPlanId.HasValue)
        {
            subscription.MealPlanId = request.MealPlanId.Value;
        }

        if (!string.IsNullOrWhiteSpace(request.PlanVariant))
        {
            subscription.PlanVariant = request.PlanVariant;
        }

        if (request.CalorieTarget.HasValue)
        {
            subscription.CalorieTarget = request.CalorieTarget;
            subscription.AverageCalories = request.CalorieTarget.Value;
        }

        if (request.MealSlots.Count > 0)
        {
            subscription.SelectedMealTypes = request.MealSlots
                .Where(x => x.Enabled)
                .Select(x => new MealTypeSelection
                {
                    MealTypeId = x.MealTypeId,
                    QuantityPerDay = x.QuantityPerDay,
                    Price = newMealTypeInputs.FirstOrDefault(m => m.MealTypeId == x.MealTypeId)?.Price ?? 0,
                    MealTypeNameEn = newMealTypeInputs.FirstOrDefault(m => m.MealTypeId == x.MealTypeId)?.MealTypeNameEn,
                    MealTypeNameAr = newMealTypeInputs.FirstOrDefault(m => m.MealTypeId == x.MealTypeId)?.MealTypeNameAr
                })
                .ToList();
        }

        subscription.TotalCost = newPricing.TotalCost;
        subscription.VatAmount = newPricing.VatAmount;
        subscription.AverageCalories = newPricing.AverageCalories;

        await subscriptionRepo.UpdateAsync(subscription, cancellationToken);

        var selections = await selectionReadRepo.ListAsync(
            new ExpressionSpecification<DailySelection>(x =>
                x.ClientId == request.ClientId &&
                x.ClientSubscriptionId == subscription.Id &&
                (request.ApplyStartingFromThisDay ? x.Date >= request.Date : x.Date == request.Date)),
            cancellationToken);

        foreach (var selection in selections)
        {
            if (request.MealPlanId.HasValue)
            {
                selection.MealPlanId = request.MealPlanId.Value;
            }

            await selectionWriteRepo.UpdateAsync(selection, cancellationToken);
        }

        var newMealPlan = await mealPlanRepo.GetByIdAsync(subscription.MealPlanId, cancellationToken);
        var newPlanLabel =
            $"{subscription.CalorieTarget ?? (int)subscription.AverageCalories} kcal {subscription.PlanVariant ?? newMealPlan?.NameEn}";

        await ClientAccountActivityLogHelper.LogAsync(
            activityLogRepo, currentUser, request.ClientId,
            ClientAccountActivityAction.MealPlanChanged,
            previousPlanLabel,
            newPlanLabel,
            request.Date,
            cancellationToken: cancellationToken);

        var dto = await ClientDeliveryCalendarHelper.BuildDayDetailAsync(
            request.ClientId, request.Date, subscription,
            selectionReadRepo, mealSelectionReadRepo, mealTypeRepo, mealPlanRepo, paymentRepo, mealRepo, settings,
            paymentResult, cancellationToken);

        return await Result<ClientDeliveryDayDetailDto>.SuccessAsync(dto);
    }
}

public class ReplaceClientDeliveryMealsRequest : ICommand<Result<ClientDeliveryDayDetailDto>>
{
    public DefaultIdType ClientId { get; set; }
    public DateOnly Date { get; set; }
    public List<ClientDeliveryMealReplacementDto> Replacements { get; set; } = [];
    public string? PaymentMethodId { get; set; }
    public bool WaivePayment { get; set; }
}

internal sealed class DeliveryCalendarDailyMenuMealByIdSpec : Specification<DailyMenuMeal>
{
    public DeliveryCalendarDailyMenuMealByIdSpec(DefaultIdType id)
    {
        Query.Where(p => p.Id == id).Include(x => x.DailyMenu).Include(x => x.Meal);
    }
}

public class ReplaceClientDeliveryMealsRequestHandler(
    ICurrentUser currentUser,
    IReadRepository<Client> clientRepo,
    IReadRepository<ClientSubscription> subscriptionRepo,
    IReadRepository<DailySelection> selectionReadRepo,
    IRepository<DailyMealSelection> mealSelectionRepo,
    IReadRepository<DailyMealSelection> mealSelectionReadRepo,
    IRepository<DailySelection> dailySelectionWriteRepo,
    IReadRepository<Meal> mealRepo,
    IReadRepository<DailyMenuMeal> dailyMenuMealRepo,
    IReadRepository<MealType> mealTypeRepo,
    IReadRepository<MealPlan> mealPlanRepo,
    IReadRepository<Payment> paymentRepo,
    IRepository<Payment> paymentWriteRepo,
    IReadRepository<ClientPaymentMethod> clientPaymentMethodRepo,
    IRepository<ClientLoyaltyPoint> loyaltyRepo,
    IRepository<ClientAccountActivityLog> activityLogRepo,
    IStripeService stripeService,
    IClientPortalSettingservice settings,
    IStringLocalizer<ReplaceClientDeliveryMealsRequestHandler> localizer)
    : ICommandHandler<ReplaceClientDeliveryMealsRequest, Result<ClientDeliveryDayDetailDto>>
{
    public async Task<Result<ClientDeliveryDayDetailDto>> Handle(
        ReplaceClientDeliveryMealsRequest request,
        CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);
        var cutoff = await ClientDeliveryCalendarHelper.BuildCutoffSettingsAsync(settings, cancellationToken);
        ClientDeliveryCalendarHelper.EnsureCutoffAllowsChange(request.Date, cutoff, localizer);

        var (client, subscription) = await ClientDeliveryCalendarHelper.GetActiveSubscriptionAsync(
            clientRepo, subscriptionRepo, request.ClientId, localizer, cancellationToken);

        decimal totalUpgradeCost = 0;
        var upgradeDescriptions = new List<string>();
        var pendingUpdates = new List<(DailyMealSelection Entity, DailyMenuMeal DailyMenuMeal, Meal Meal, decimal UpgradeCost)>();

        foreach (var replacement in request.Replacements)
        {
            var entity = await mealSelectionRepo.GetByIdAsync(replacement.DailyMealSelectionId, cancellationToken)
                ?? throw new NotFoundException(localizer["Daily meal selection not found"]);

            if (entity.ClientId != request.ClientId)
            {
                throw new ForbiddenException(localizer["Meal does not belong to this client."]);
            }

            var dailyMenuMeal = await dailyMenuMealRepo.FirstOrDefaultAsync(
                new DeliveryCalendarDailyMenuMealByIdSpec(replacement.DailyMenuMealId), cancellationToken)
                ?? throw new NotFoundException(localizer["The meal option you selected isn't available. Please choose another."]);

            var meal = dailyMenuMeal.Meal
                ?? await mealRepo.GetByIdAsync(replacement.MealId, cancellationToken)
                ?? throw new NotFoundException(localizer["Meal not found"]);

            if (meal.Id != replacement.MealId)
            {
                throw new BadRequestException(localizer["Meal does not match the selected menu option."]);
            }

            var upgradeCost = ClientDeliveryCalendarHelper.CalculateMealUpgradeCost(entity, dailyMenuMeal);
            if (upgradeCost > 0)
            {
                totalUpgradeCost += upgradeCost;
                upgradeDescriptions.Add($"{meal.NameEn} (+{upgradeCost})");
            }

            pendingUpdates.Add((entity, dailyMenuMeal, meal, upgradeCost));
        }

        var paymentMethodId = await ClientDeliveryCalendarPaymentHelper.ResolvePaymentMethodIdAsync(
            request.ClientId, request.PaymentMethodId, request.WaivePayment, totalUpgradeCost,
            clientPaymentMethodRepo, localizer, cancellationToken);

        var paymentDescription = upgradeDescriptions.Count == 0
            ? $"Meal replacements for {request.Date:yyyy-MM-dd}"
            : $"Meal upgrades for {request.Date:yyyy-MM-dd}: {string.Join(", ", upgradeDescriptions)}";

        var paymentResult = await ClientDeliveryCalendarPaymentHelper.ChargeAsync(
            client, subscription, totalUpgradeCost, paymentDescription,
            request.Date.ToString("yyyyMMdd"), paymentMethodId, request.WaivePayment,
            stripeService, paymentWriteRepo, settings, loyaltyRepo, localizer, cancellationToken);

        foreach (var (entity, dailyMenuMeal, meal, upgradeCost) in pendingUpdates)
        {
            var dailySelection = await dailySelectionWriteRepo.GetByIdAsync(entity.DailySelectionId, cancellationToken)
                ?? throw new NotFoundException(localizer["Daily selection not found"]);

            dailySelection.TotalCalories -= entity.TotalCalories;
            dailySelection.TotalProteins -= entity.TotalProteins;
            dailySelection.TotalFats -= entity.TotalFats;
            dailySelection.TotalCarbohydrates -= entity.TotalCarbohydrates;

            entity.MealId = meal.Id;
            entity.CustomMealId = null;
            entity.MealSelectionType = MealSelectionType.Default;
            entity.MealPlanId = dailyMenuMeal.DailyMenu?.MealPlanId ?? entity.MealPlanId;
            entity.TotalCalories = meal.Calories;
            entity.TotalProteins = meal.Protein;
            entity.TotalFats = meal.Fat;
            entity.TotalCarbohydrates = meal.Carbs;
            entity.CustomeMealName = meal.NameEn;

            if (upgradeCost > 0)
            {
                entity.AdjustedPrice = dailyMenuMeal.DailyMenu!.Price;
                entity.PriceAdjustmentReason = $"Meal upgraded (+{upgradeCost}) to {meal.NameEn}";
                entity.IsPaid = paymentResult.AmountCharged > 0 || request.WaivePayment;
            }

            dailySelection.TotalCalories += entity.TotalCalories;
            dailySelection.TotalProteins += entity.TotalProteins;
            dailySelection.TotalFats += entity.TotalFats;
            dailySelection.TotalCarbohydrates += entity.TotalCarbohydrates;

            await mealSelectionRepo.UpdateAsync(entity, cancellationToken);
            await dailySelectionWriteRepo.UpdateAsync(dailySelection, cancellationToken);
        }

        await ClientAccountActivityLogHelper.LogAsync(
            activityLogRepo, currentUser, request.ClientId,
            ClientAccountActivityAction.MealsReplaced,
            request.Date.ToString("yyyy-MM-dd"),
            upgradeDescriptions.Count == 0
                ? $"{request.Replacements.Count} meal(s) replaced"
                : string.Join(", ", upgradeDescriptions),
            request.Date,
            cancellationToken: cancellationToken);

        var dto = await ClientDeliveryCalendarHelper.BuildDayDetailAsync(
            request.ClientId, request.Date, subscription,
            selectionReadRepo, mealSelectionReadRepo, mealTypeRepo, mealPlanRepo, paymentRepo, mealRepo, settings,
            paymentResult, cancellationToken);

        return await Result<ClientDeliveryDayDetailDto>.SuccessAsync(dto);
    }
}

public class PreviewClientDeliveryAddItemsRequest : IQuery<Result<ClientDeliveryAddOnPreviewDto>>
{
    public DefaultIdType ClientId { get; set; }
    public DateOnly Date { get; set; }
    public List<ClientDeliveryAddOnItemDto> Items { get; set; } = [];
}

public class PreviewClientDeliveryAddItemsRequestHandler(
    ICurrentUser currentUser,
    IReadRepository<Client> clientRepo,
    IReadRepository<ClientSubscription> subscriptionRepo,
    IReadRepository<Meal> mealRepo,
    IStringLocalizer<PreviewClientDeliveryAddItemsRequestHandler> localizer)
    : IQueryHandler<PreviewClientDeliveryAddItemsRequest, Result<ClientDeliveryAddOnPreviewDto>>
{
    public async Task<Result<ClientDeliveryAddOnPreviewDto>> Handle(
        PreviewClientDeliveryAddItemsRequest request,
        CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);
        _ = await ClientDeliveryCalendarHelper.GetActiveSubscriptionAsync(
            clientRepo, subscriptionRepo, request.ClientId, localizer, cancellationToken);

        var lines = new List<ClientDeliveryAddOnPreviewLineDto>();
        decimal total = 0;

        foreach (var item in request.Items)
        {
            var meal = await mealRepo.GetByIdAsync(item.MealId, cancellationToken)
                ?? throw new NotFoundException(localizer["Meal not found"]);

            if (!meal.IsAddOn || meal.PriceForCustomer <= 0)
            {
                throw new BadRequestException(localizer["Only add-on meals can be added."]);
            }

            var lineTotal = (decimal)meal.PriceForCustomer * item.Quantity;
            total += lineTotal;
            lines.Add(new ClientDeliveryAddOnPreviewLineDto
            {
                MealId = meal.Id,
                MealName = meal.NameEn,
                Quantity = item.Quantity,
                UnitPrice = (decimal)meal.PriceForCustomer,
                LineTotal = lineTotal
            });
        }

        return await Result<ClientDeliveryAddOnPreviewDto>.SuccessAsync(new ClientDeliveryAddOnPreviewDto
        {
            Date = request.Date,
            Lines = lines,
            TotalPrice = total,
            RequiresPayment = total > 0
        });
    }
}

public class PreviewClientDeliveryReplaceMealsRequest : IQuery<Result<ClientDeliveryReplaceMealsPreviewDto>>
{
    public DefaultIdType ClientId { get; set; }
    public DateOnly Date { get; set; }
    public List<ClientDeliveryMealReplacementDto> Replacements { get; set; } = [];
}

public class PreviewClientDeliveryReplaceMealsRequestHandler(
    ICurrentUser currentUser,
    IReadRepository<Client> clientRepo,
    IReadRepository<ClientSubscription> subscriptionRepo,
    IRepository<DailyMealSelection> mealSelectionRepo,
    IReadRepository<DailyMenuMeal> dailyMenuMealRepo,
    IReadRepository<MealType> mealTypeRepo,
    IStringLocalizer<PreviewClientDeliveryReplaceMealsRequestHandler> localizer)
    : IQueryHandler<PreviewClientDeliveryReplaceMealsRequest, Result<ClientDeliveryReplaceMealsPreviewDto>>
{
    public async Task<Result<ClientDeliveryReplaceMealsPreviewDto>> Handle(
        PreviewClientDeliveryReplaceMealsRequest request,
        CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);
        _ = await ClientDeliveryCalendarHelper.GetActiveSubscriptionAsync(
            clientRepo, subscriptionRepo, request.ClientId, localizer, cancellationToken);

        var lines = new List<ClientDeliveryReplaceMealPreviewLineDto>();
        decimal totalUpgrade = 0;

        foreach (var replacement in request.Replacements)
        {
            var entity = await mealSelectionRepo.GetByIdAsync(replacement.DailyMealSelectionId, cancellationToken)
                ?? throw new NotFoundException(localizer["Daily meal selection not found"]);

            if (entity.ClientId != request.ClientId)
            {
                throw new ForbiddenException(localizer["Meal does not belong to this client."]);
            }

            var dailyMenuMeal = await dailyMenuMealRepo.FirstOrDefaultAsync(
                new DeliveryCalendarDailyMenuMealByIdSpec(replacement.DailyMenuMealId), cancellationToken)
                ?? throw new NotFoundException(localizer["The meal option you selected isn't available. Please choose another."]);

            var upgradeCost = ClientDeliveryCalendarHelper.CalculateMealUpgradeCost(entity, dailyMenuMeal);
            totalUpgrade += upgradeCost;

            string? mealTypeName = null;
            if (entity.MealTypeId.HasValue)
            {
                var mealType = await mealTypeRepo.GetByIdAsync(entity.MealTypeId.Value, cancellationToken);
                mealTypeName = mealType?.NameEn;
            }

            lines.Add(new ClientDeliveryReplaceMealPreviewLineDto
            {
                DailyMealSelectionId = entity.Id,
                MealTypeName = mealTypeName,
                CurrentMealName = entity.CustomeMealName,
                NewMealName = dailyMenuMeal.Meal?.NameEn,
                BasePrice = entity.BasePrice,
                NewMenuPrice = dailyMenuMeal.DailyMenu?.Price ?? 0,
                UpgradeCost = upgradeCost
            });
        }

        return await Result<ClientDeliveryReplaceMealsPreviewDto>.SuccessAsync(new ClientDeliveryReplaceMealsPreviewDto
        {
            Date = request.Date,
            Lines = lines,
            TotalUpgradeCost = totalUpgrade,
            RequiresPayment = totalUpgrade > 0
        });
    }
}

public class PreviewClientDeliveryMealPlanChangeRequest : IQuery<Result<ClientDeliveryMealPlanChangePreviewDto>>
{
    public DefaultIdType ClientId { get; set; }
    public DateOnly Date { get; set; }
    public DefaultIdType? MealPlanId { get; set; }
    public List<ClientDeliveryMealPlanSlotDto> MealSlots { get; set; } = [];
    public bool ApplyStartingFromThisDay { get; set; }
}

public class PreviewClientDeliveryMealPlanChangeRequestHandler(
    ICurrentUser currentUser,
    IReadRepository<Client> clientRepo,
    IReadRepository<ClientSubscription> subscriptionRepo,
    IReadRepository<MealPlanMealType> mealPlanMealTypeRepo,
    ISubscriptionPricingService pricingService,
    IStringLocalizer<PreviewClientDeliveryMealPlanChangeRequestHandler> localizer)
    : IQueryHandler<PreviewClientDeliveryMealPlanChangeRequest, Result<ClientDeliveryMealPlanChangePreviewDto>>
{
    public async Task<Result<ClientDeliveryMealPlanChangePreviewDto>> Handle(
        PreviewClientDeliveryMealPlanChangeRequest request,
        CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);
        var (_, subscription) = await ClientDeliveryCalendarHelper.GetActiveSubscriptionAsync(
            clientRepo, subscriptionRepo, request.ClientId, localizer, cancellationToken);

        var oldPricing = await pricingService.CalculateAsync(
            ClientDeliveryCalendarPricingHelper.BuildPricingInput(subscription), cancellationToken);

        var targetMealPlanId = request.MealPlanId ?? subscription.MealPlanId;
        var mealSlots = request.MealSlots.Count > 0
            ? request.MealSlots
            : subscription.SelectedMealTypes.Select(x => new ClientDeliveryMealPlanSlotDto
            {
                MealTypeId = x.MealTypeId,
                Enabled = true,
                QuantityPerDay = x.QuantityPerDay
            }).ToList();

        var newMealTypeInputs = await ClientDeliveryCalendarPricingHelper.BuildMealTypePricingInputsAsync(
            targetMealPlanId, mealSlots, mealPlanMealTypeRepo, cancellationToken);

        var newPricingInput = ClientDeliveryCalendarPricingHelper.BuildPricingInput(subscription);
        newPricingInput.MealTypeSelections = newMealTypeInputs;
        var newPricing = await pricingService.CalculateAsync(newPricingInput, cancellationToken);

        var periodDiff = newPricing.TotalCost - oldPricing.TotalCost;
        var proratedCharge = ClientDeliveryCalendarPricingHelper.CalculateProratedUpgradeAmount(
            subscription, oldPricing.TotalCost, newPricing.TotalCost);

        return await Result<ClientDeliveryMealPlanChangePreviewDto>.SuccessAsync(new ClientDeliveryMealPlanChangePreviewDto
        {
            Date = request.Date,
            CurrentPlanTotal = oldPricing.TotalCost,
            NewPlanTotal = newPricing.TotalCost,
            PeriodDifference = periodDiff,
            ProratedChargeAmount = proratedCharge,
            RequiresPayment = proratedCharge > 0,
            ApplyStartingFromThisDay = request.ApplyStartingFromThisDay
        });
    }
}
