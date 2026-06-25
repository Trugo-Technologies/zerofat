using ZeroFat.Application.Common.Interfaces;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.Contracts;

/// <summary>Single day in delivery calendar — Status drives UI coloring.</summary>
public class DeliveryCalendarDayDto
{
    public DateOnly Date { get; set; }
    public string Status { get; set; } = "Available";
}

public class DeliveryCalendarResult
{
    public List<DeliveryCalendarDayDto> Days { get; set; } = [];
    public DateOnly SuggestedStartDate { get; set; }
    public DateOnly? ActiveSubscriptionEndDate { get; set; }
    public bool HasActiveSubscription { get; set; }
}

public class DeliveryCalendarInput
{
    public DefaultIdType ClientId { get; set; }
    public SubscriptionType SubscriptionType { get; set; }
    public bool SkipSaturdays { get; set; }
    public bool SkipSundays { get; set; }
    public SubscriptionRenewalStrategy? RenewalStrategy { get; set; }
    public DateOnly? ScheduledStartDate { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
}

/// <summary>Calendar month view for wizard schedule step (GET wizard/{draftId}/delivery-calendar).</summary>
public interface IDeliveryCalendarService : ITransientService
{
    Task<DeliveryCalendarResult> BuildCalendarAsync(DeliveryCalendarInput input, CancellationToken cancellationToken = default);
    List<DayOfWeek> DeriveDeliveryDays(bool skipSaturdays, bool skipSundays);
    List<DateOnly> BuildDeliveryDates(DateOnly startDate, DateOnly endDate, bool skipSaturdays, bool skipSundays);
}
