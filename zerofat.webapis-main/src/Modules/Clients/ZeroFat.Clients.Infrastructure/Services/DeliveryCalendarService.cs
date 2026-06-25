using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Infrastructure.Services;

/// <summary>
/// Delivery calendar for admin wizard schedule step.
/// Day statuses returned to UI: Available, ActiveSubs, RenewalSelected, Selected, Unavailable.
/// Respects skip-Saturday/Sunday flags and active subscription overlap.
/// </summary>
internal sealed class DeliveryCalendarService(
    IReadRepository<Client> clientRepo,
    IReadRepository<ClientSubscription> subscriptionRepo,
    ISubscriptionPricingService pricingService) : IDeliveryCalendarService
{
    public List<DayOfWeek> DeriveDeliveryDays(bool skipSaturdays, bool skipSundays)
    {
        var days = Enum.GetValues<DayOfWeek>().ToList();
        if (skipSaturdays)
        {
            days.Remove(DayOfWeek.Saturday);
        }

        if (skipSundays)
        {
            days.Remove(DayOfWeek.Sunday);
        }

        return days;
    }

    public List<DateOnly> BuildDeliveryDates(DateOnly startDate, DateOnly endDate, bool skipSaturdays, bool skipSundays)
    {
        var dates = new List<DateOnly>();
        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            if (skipSaturdays && date.DayOfWeek == DayOfWeek.Saturday)
            {
                continue;
            }

            if (skipSundays && date.DayOfWeek == DayOfWeek.Sunday)
            {
                continue;
            }

            dates.Add(date);
        }

        return dates;
    }

    public async Task<DeliveryCalendarResult> BuildCalendarAsync(
        DeliveryCalendarInput input,
        CancellationToken cancellationToken = default)
    {
        var client = await clientRepo.FirstOrDefaultAsync(
            new ExpressionSpecification<Client>(x => x.Id == input.ClientId),
            cancellationToken);

        ClientSubscription? activeSubscription = null;
        if (client?.ClientSubscriptionId != null && client.SubscriptionStatus == SubscriptionStatus.Active)
        {
            activeSubscription = await subscriptionRepo.FirstOrDefaultAsync(
                new ExpressionSpecification<ClientSubscription>(x => x.Id == client.ClientSubscriptionId),
                cancellationToken);
        }

        var suggestedStart = input.ScheduledStartDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        if (activeSubscription != null)
        {
            suggestedStart = input.RenewalStrategy == SubscriptionRenewalStrategy.ScheduleFutureDate
                ? input.ScheduledStartDate ?? activeSubscription.EndDate.AddDays(1)
                : activeSubscription.EndDate.AddDays(1);
        }

        var endDate = pricingService.GetDurationRules(input.SubscriptionType, suggestedStart).EndDate;

        var monthStart = new DateOnly(input.Year, input.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);
        var days = new List<DeliveryCalendarDayDto>();

        for (var date = monthStart; date <= monthEnd; date = date.AddDays(1))
        {
            var status = "Available";
            if (activeSubscription != null && date >= activeSubscription.StartDate && date <= activeSubscription.EndDate)
            {
                status = "ActiveSubs";
            }
            else if (date >= suggestedStart && date <= endDate &&
                     !((input.SkipSaturdays && date.DayOfWeek == DayOfWeek.Saturday) ||
                       (input.SkipSundays && date.DayOfWeek == DayOfWeek.Sunday)))
            {
                status = input.RenewalStrategy == SubscriptionRenewalStrategy.ExtendAfterExpiry &&
                         activeSubscription != null
                    ? "RenewalSelected"
                    : "Selected";
            }
            else if (date < suggestedStart)
            {
                status = "Unavailable";
            }

            days.Add(new DeliveryCalendarDayDto { Date = date, Status = status });
        }

        return new DeliveryCalendarResult
        {
            Days = days,
            SuggestedStartDate = suggestedStart,
            ActiveSubscriptionEndDate = activeSubscription?.EndDate,
            HasActiveSubscription = activeSubscription != null
        };
    }
}
