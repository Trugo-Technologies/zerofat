using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Application.NutritionTracking.DailyHealthLogs;
using ZeroFat.ClientPortal.Domain.Common.ValueObjects;
using ZeroFat.ClientPortal.Domain.NutritionTracking;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.ClientPortal.Infrastructure.Persistence.Context;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.ClientPortal.Infrastructure.Services;

public class WeightProgressService : IWeightProgressService
{
    private readonly ClientPortalContext _context;
    private const int MaxDataPoints = 10;

    // Projection days for each timeframe
    private readonly Dictionary<StatisticsType, int> _projectionDays = new()
    {
        { StatisticsType.W, 4 },    // Week: show next 7 days
        { StatisticsType.M, 15 },   // Month: show next 21 days
        { StatisticsType.M3, 42 },  // 3 Months: show next 6 weeks
        { StatisticsType.M6, 84 },  // 6 Months: show next 12 weeks
        { StatisticsType.Y, 168 }   // Year: show next 24 weeks
    };

    public WeightProgressService(ClientPortalContext context)
    {
        _context = context;
    }

    public async Task<WeightProgressReport> GetWeightProgressReportAsync(StatisticsType timeframe, Guid userId)
    {
        var report = new WeightProgressReport();
        var now = DateTime.UtcNow;
        var today = DateOnly.FromDateTime(now);

        // Get client and their weight logs
        var client = await _context.Clients
            .Where(c => c.Id == userId).AsNoTracking().FirstOrDefaultAsync();

        if (client == null) return report;

        // Set basic report info
        report.StartingWeight = client.WeightInKG;
        report.TargetWeight = client.TargetWeightInKG;
        report.CurrentWeight = client.CurrentWeightInKG;
        report.GoalStartDate = client.NewGoalStart;
        report.TimeToReachGoalInDays = client.TimeToReachGoalInDays;

        // Calculate date range (1 year history + future projection)
        var projectionDays = _projectionDays[timeframe];
        var startDate = today.AddDays(-projectionDays); // Show max 1 year history
        var endDate = today.AddDays(projectionDays);

        // Get all weight logs (we'll filter dates later)
        var weightLogs = await _context.DailyHealthLogs
            .Where(log => log.ClientId == userId && log.Weight.Value > 0)
            .OrderBy(log => log.LogDate)
            .AsNoTracking()
            .Select(log => new DailyHealthLogSimplifyDto
            {
                Weight = log.Weight,
                LogDate = log.LogDate
            })
            .ToListAsync();

        // Process data points
        var processedData = ProcessWeightData(report, weightLogs, startDate, endDate, timeframe);
        report.DataPoints = DownsampleData(processedData, MaxDataPoints);

        // Calculate goal progress if possible
        if (client.NewGoalStart.HasValue &&
            client.WeightInKG.HasValue &&
            client.TargetWeightInKG.HasValue &&
            client.CurrentWeightInKG.HasValue)
        {
            CalculateGoalProgress(report, client, now);
        }

        return report;
    }

    private List<WeightDataPoint> ProcessWeightData(
        WeightProgressReport report,
        List<DailyHealthLogSimplifyDto> weightLogs,
        DateOnly startDate,
        DateOnly endDate,
        StatisticsType timeframe)
    {
        var result = new List<WeightDataPoint>();

        if (!weightLogs.Any())
            return CreateDefaultProjections(report, startDate, endDate, timeframe);

        // Add all actual data points within date range
        foreach (var log in weightLogs.Where(log => log.LogDate >= startDate && log.LogDate <= endDate))
        {
            result.Add(new WeightDataPoint
            {
                Date = log.LogDate,
                Value = log.Weight?.Value,
                IsEstimated = false,
                Status = WeightTrendStatus.OnTrack
            });
        }

        // Fill gaps in historical data
        if (result.Any())
        {
            result = FillHistoricalGaps(result, startDate, endDate, timeframe);
        }

        // Add future projections if we have any actual data
        if (result.Any(dp => !dp.IsEstimated))
        {
            var lastActualDate = result.Where(dp => !dp.IsEstimated).Max(dp => dp.Date);
            var projectionStartDate = lastActualDate.AddDays(1);

            if (projectionStartDate <= endDate)
            {
                result.AddRange(CreateDefaultProjections(report, projectionStartDate, endDate, timeframe));
            }
        }
        else
        {
            // If no actual data, create default projections
            result.AddRange(CreateDefaultProjections(report, startDate, endDate, timeframe));
        }

        return result.OrderBy(dp => dp.Date).ToList();
    }

    private List<WeightDataPoint> FillHistoricalGaps(
    List<WeightDataPoint> data,
    DateOnly startDate,
    DateOnly endDate,
    StatisticsType timeframe)
    {
        var filledData = new List<WeightDataPoint>(data);
        var interval = GetSamplingInterval(timeframe);
        var currentDate = startDate;

        // Avoid interpolation before the earliest recorded weight
        var earliestRecordedDate = data.Min(dp => dp.Date);

        while (currentDate <= endDate && currentDate <= DateOnly.FromDateTime(DateTime.UtcNow))
        {
            if (!filledData.Any(dp => dp.Date == currentDate))
            {
                var previous = data.LastOrDefault(dp => dp.Date <= currentDate);
                var next = data.FirstOrDefault(dp => dp.Date >= currentDate);

                double? estimatedValue = null;

                if (previous != null && next != null)
                {
                    // Linear interpolation
                    var daysBetween = next.Date.DayNumber - previous.Date.DayNumber;
                    var daysToCurrent = currentDate.DayNumber - previous.Date.DayNumber;

                    if (daysBetween > 0 && previous.Value.HasValue && next.Value.HasValue)
                    {
                        var weightDiff = next.Value.Value - previous.Value.Value;
                        estimatedValue = previous.Value.Value + (weightDiff * daysToCurrent / daysBetween);
                    }
                }
                else if (previous != null)
                {
                    // If there's no "next" value, use the previous value
                    estimatedValue = previous.Value;
                }
                else if (next != null)
                {
                    // If there's no "previous" value, use the next value
                    estimatedValue = next.Value;
                }

                // Ensure that we're not projecting back in time to unrealistic weights
                if (estimatedValue.HasValue && currentDate >= earliestRecordedDate)
                {
                    filledData.Add(new WeightDataPoint
                    {
                        Date = currentDate,
                        Value = estimatedValue,
                        IsEstimated = true,
                        Status = WeightTrendStatus.Estimated
                    });
                }
            }

            currentDate = currentDate.AddDays(interval);
        }

        return filledData;
    }

    private List<WeightDataPoint> CreateDefaultProjections(
     WeightProgressReport report,
     DateOnly startDate,
     DateOnly endDate,
     StatisticsType timeframe)
    {
        var projections = new List<WeightDataPoint>();
        var interval = GetSamplingInterval(timeframe);
        var currentDate = startDate;

        // Extract useful values
        double? startWeight = report.StartingWeight;
        double? targetWeight = report.TargetWeight;
        double? currentWeight = report.CurrentWeight;
        DateOnly? goalStart = report.GoalStartDate;
        int duration = report.TimeToReachGoalInDays;

        // Determine if goal info is valid
        bool goalDefined = startWeight.HasValue && targetWeight.HasValue && goalStart.HasValue && duration > 0;
        bool goalAchieved = currentWeight.HasValue && goalDefined && (
            (startWeight > targetWeight && currentWeight <= targetWeight) ||
            (startWeight < targetWeight && currentWeight >= targetWeight)
        );

        if (!goalDefined)
        {
            // No goal info: flat projection with current weight
            while (currentDate <= endDate)
            {
                projections.Add(new WeightDataPoint
                {
                    Date = currentDate,
                    Value = currentWeight,
                    IsEstimated = true,
                    Status = WeightTrendStatus.Estimated
                });

                currentDate = currentDate.AddDays(interval);
            }

            return projections;
        }

        var goalEnd = goalStart.Value.AddDays(duration);
        double totalChange = targetWeight.Value - startWeight.Value;
        double dailyChange = duration != 0 ? totalChange / duration : 0;

        while (currentDate <= endDate)
        {
            double? value;

            if (goalAchieved)
            {
                // Goal already achieved: maintain target weight
                value = targetWeight;
            }
            else if (currentDate < goalStart.Value)
            {
                // Before goal starts: hold current weight
                value = currentWeight;
            }
            else if (currentDate <= goalEnd)
            {
                // During goal: interpolate between start and target
                var daysSinceStart = currentDate.DayNumber - goalStart.Value.DayNumber;
                value = startWeight.Value + daysSinceStart * dailyChange;
            }
            else
            {
                // After goal deadline: maintain target weight
                value = targetWeight;
            }

            projections.Add(new WeightDataPoint
            {
                Date = currentDate,
                Value = value,
                IsEstimated = true,
                Status = WeightTrendStatus.Estimated
            });

            currentDate = currentDate.AddDays(interval);
        }

        return projections;
    }

    private void CalculateGoalProgress(WeightProgressReport report, Client client, DateTime now)
    {
        var startDate = client.NewGoalStart.Value.ToDateTime(TimeOnly.MinValue);
        var elapsedDays = (now - startDate).TotalDays;

        var initialWeight = client.WeightInKG ?? 0;
        var currentWeight = client.CurrentWeightInKG ?? 0;
        var targetWeight = client.TargetWeightInKG ?? 0;
        var totalDuration = client.TimeToReachGoalInDays;

        if (totalDuration <= 0 || initialWeight == 0 || targetWeight == 0)
        {

            report.WeightToLose = 0;
            report.DailyRateRequired = 0;
            report.DaysRemaining = 0;
            report.ProjectedGoalDate = now.AddDays(0);
            return;
        }
           

        // Required daily rate to hit target
        var totalWeightChange = targetWeight - initialWeight;
        var requiredDailyRate = totalWeightChange / totalDuration;

        // Actual progress so far
        var actualWeightChange = currentWeight - initialWeight;
        var expectedWeightChange = requiredDailyRate * elapsedDays;

        // Project remaining days (avoid div by zero)
        double remainingDays = requiredDailyRate != 0
            ? (targetWeight - currentWeight) / requiredDailyRate
            : 0;

        // Sanitize remaining days
        if (remainingDays < 0) remainingDays = 0;
        var roundedRemainingDays = (int)Math.Round(remainingDays);

        // Update report
        report.WeightToLose = targetWeight - currentWeight;
        report.DailyRateRequired = requiredDailyRate;
        report.DaysRemaining = roundedRemainingDays;
        report.ProjectedGoalDate = now.AddDays(roundedRemainingDays);

        // === Trend Evaluation ===

        // Use percentage-based tolerance
        var expected = expectedWeightChange == 0 ? 0.01 : expectedWeightChange;
        var deviation = Math.Abs(actualWeightChange - expected);
        var deviationPercent = deviation / Math.Abs(expected);

        // Dynamic tolerance: more strict near the end
        double progressRatio = elapsedDays / totalDuration;
        var tolerance = progressRatio < 0.5 ? 0.15 : 0.08;

        report.TrendStatus = (deviationPercent <= tolerance)
            ? WeightTrendStatus.OnTrack
            : WeightTrendStatus.OffTrack;

        // Mark data points with current trend
        foreach (var point in report.DataPoints.Where(dp => !dp.IsEstimated))
        {
            point.Status = report.TrendStatus;
        }

        // ⏳ Update total duration if the goal is achievable sooner
        if (roundedRemainingDays + elapsedDays < totalDuration)
        {
            client.TimeToReachGoalInDays = (int)Math.Round(elapsedDays + remainingDays);
        }
    }

    private List<WeightDataPoint> DownsampleData(List<WeightDataPoint> data, int maxPoints)
    {
        if (data.Count <= maxPoints)
            return data;

        var result = new List<WeightDataPoint>
        {
            // Always include the first and last data points
            data.First(),
            data.Last()
        };

        var remainingSlots = maxPoints - 2;

        // Get key extrema: peaks and troughs
        var extrema = new List<WeightDataPoint>();
        for (int i = 1; i < data.Count - 1; i++)
        {
            var prev = data[i - 1].Value;
            var curr = data[i].Value;
            var next = data[i + 1].Value;

            if (curr.HasValue && prev.HasValue && next.HasValue)
            {
                if ((curr > prev && curr > next) || (curr < prev && curr < next))
                {
                    extrema.Add(data[i]);
                }
            }
        }

        // Take most significant extrema (limit to available slots)
        var topExtrema = extrema
            .OrderByDescending(p => Math.Abs(p.Value!.Value - data.Average(x => x.Value ?? 0)))
            .Take(remainingSlots)
            .ToList();

        result.AddRange(topExtrema);
        remainingSlots -= topExtrema.Count;

        // Fill the rest with evenly spaced points
        if (remainingSlots > 0)
        {
            var step = (double)data.Count / (remainingSlots + 2); // +2 for first and last
            for (int i = 1; i <= remainingSlots; i++)
            {
                int index = (int)Math.Round(i * step);
                if (index >= data.Count - 1) break;
                var point = data[index];

                if (!result.Any(p => p.Date == point.Date))
                {
                    result.Add(point);
                }
            }
        }

        return result.OrderBy(p => p.Date).ToList();
    }


    private int GetSamplingInterval(StatisticsType timeframe)
    {
        return timeframe switch
        {
            StatisticsType.W => 1,       // Daily for week
            StatisticsType.M => 3,       // Every 3 days for month
            StatisticsType.M3 => 7,     // Weekly for 3 months
            StatisticsType.M6 => 14,     // Bi-weekly for 6 months
            StatisticsType.Y => 30,      // Monthly for year
            _ => 1                      // Default to daily
        };
    }
}

