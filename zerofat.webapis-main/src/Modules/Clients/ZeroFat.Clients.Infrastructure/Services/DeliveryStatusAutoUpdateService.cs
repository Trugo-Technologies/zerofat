using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TimeZoneConverter;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Infrastructure.Persistence.Context;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Infrastructure.Services;

/// <summary>
/// Marks daily selections as Delivered once their preferred delivery window has ended (UAE time).
/// EarlyMorning 6–8 AM → after 8:00 AM | Morning 8–10 AM → after 10:00 AM | Evening 6–8 PM → after 8:00 PM
/// </summary>
public class DeliveryStatusAutoUpdateService(
    ClientPortalContext dbContext,
    ILogger<DeliveryStatusAutoUpdateService> logger) : IDeliveryStatusAutoUpdateService
{
    private static readonly TimeZoneInfo UaeTimeZone =
        TZConvert.GetTimeZoneInfo("Arabian Standard Time");

    private static readonly Dictionary<PreferredMealTime, TimeOnly> WindowEndByDeliveryTime = new()
    {
        [PreferredMealTime.EarlyMorning] = new(8, 0),
        [PreferredMealTime.Morning] = new(10, 0),
        [PreferredMealTime.Evening] = new(20, 0),
    };

    public async Task MarkDeliveriesAsDeliveredAsync(CancellationToken cancellationToken = default)
    {
        var nowUtc = DateTime.UtcNow;
        var nowUae = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, UaeTimeZone);
        var todayUae = DateOnly.FromDateTime(nowUae);

        var supportedDeliveryTimes = WindowEndByDeliveryTime.Keys.ToList();

        var pendingSelections = await dbContext.DailySelections
            .Where(s =>
                s.DailySelectionStatus == DailySelectionStatus.Pending &&
                supportedDeliveryTimes.Contains(s.DeliveryTime) &&
                s.Date <= todayUae)
            .ToListAsync(cancellationToken);

        if (pendingSelections.Count == 0)
        {
            return;
        }

        var updatedCount = 0;

        foreach (var selection in pendingSelections)
        {
            if (!HasDeliveryWindowEnded(selection.Date, selection.DeliveryTime, nowUtc))
            {
                continue;
            }

            selection.DailySelectionStatus = DailySelectionStatus.Delivered;
            updatedCount++;
        }

        if (updatedCount == 0)
        {
            return;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Marked {Count} daily selection(s) as Delivered (UAE date {Date}).",
            updatedCount,
            todayUae);
    }

    private static bool HasDeliveryWindowEnded(
        DateOnly deliveryDate,
        PreferredMealTime deliveryTime,
        DateTime nowUtc)
    {
        if (!WindowEndByDeliveryTime.TryGetValue(deliveryTime, out var windowEnd))
        {
            return false;
        }

        var windowEndLocal = deliveryDate.ToDateTime(windowEnd);
        var windowEndUtc = TimeZoneInfo.ConvertTimeToUtc(
            DateTime.SpecifyKind(windowEndLocal, DateTimeKind.Unspecified),
            UaeTimeZone);

        return nowUtc >= windowEndUtc;
    }
}
