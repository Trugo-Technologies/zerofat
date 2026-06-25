using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.Contracts;

public interface IWeightProgressService : ITransientService
{
    Task<WeightProgressReport> GetWeightProgressReportAsync(StatisticsType timeframe, Guid userId);
}



public class WeightProgressReport
{
    public List<WeightDataPoint> DataPoints { get; set; } = new();
    public double? CurrentWeight { get; set; }
    public double? TargetWeight { get; set; }
    public double? StartingWeight { get; set; }
    public DateOnly? GoalStartDate { get; set; }
    public DateTime? ProjectedGoalDate { get; set; }
    public int DaysRemaining { get; set; }
    public double? WeightToLose { get; set; }
    public double? DailyRateRequired { get; set; }
    public WeightTrendStatus TrendStatus { get; set; }
    public int TimeToReachGoalInDays { get; set; }
}

public class WeightDataPoint
{
    public DateOnly Date { get; set; }
    public double? Value { get; set; }
    public bool IsEstimated { get; set; }
    public WeightTrendStatus Status { get; set; }
}
