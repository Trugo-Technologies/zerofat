using Hangfire;
using TimeZoneConverter;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.Infrastructure.BackgroundProcessing.Contracts;

namespace ZeroFat.ClientPortal.Infrastructure.Services;

/// <summary>
/// Runs every 30 minutes (UAE time) to mark deliveries as Delivered after their time window ends.
/// </summary>
public class DeliveryStatusJobScheduler(IDeliveryStatusAutoUpdateService deliveryStatusService) : IRecurringBackgroundJobScheduler
{
    private static readonly TimeZoneInfo UaeTimeZone =
        TZConvert.GetTimeZoneInfo("Arabian Standard Time");

    public Task ScheduleAsync(IRecurringJobManager recurringJobManager)
    {
        recurringJobManager.AddOrUpdate(
            "delivery-status-auto-update",
            () => deliveryStatusService.MarkDeliveriesAsDeliveredAsync(CancellationToken.None),
            "*/30 * * * *",
            new RecurringJobOptions
            {
                TimeZone = UaeTimeZone
            });
        return Task.CompletedTask;
    }
}
