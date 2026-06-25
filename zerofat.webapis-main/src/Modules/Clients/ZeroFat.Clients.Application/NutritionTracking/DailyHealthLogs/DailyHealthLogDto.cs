using ZeroFat.Application.Common.Interfaces;
using ZeroFat.ClientPortal.Domain.Common.ValueObjects;

namespace ZeroFat.ClientPortal.Application.NutritionTracking.DailyHealthLogs;

public class DailyHealthLogSimplifyDto : IDto
{
    public BodyMeasurement? Weight { get; set; }
    public DateOnly LogDate { get; init; }
}

public class DailyHealthLogRawDto : DailyHealthLogSimplifyDto
{
    public WaterIntake? WaterConsumption { get; set; }

    public DefaultIdType Id { get; set; }

    // Persisted Computed Properties
    public double TotalCaloriesConsumed { get; set; }
    public double TotalCaloriesBurned { get; set; }
}

public class DailyHealthLogAuditableDto : DailyHealthLogRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class DailyHealthLogDto : DailyHealthLogAuditableDto
{
    public DefaultIdType ClientId { get; set; }
}

public class DailyHealthLogDetailsDto : BaseEntityAuditableDetailsDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType ClientId { get; set; }
    public BodyMeasurement? Weight { get; set; }
    public WaterIntake? WaterConsumption { get; set; }

    // Persisted Computed Properties
    public double TotalCaloriesConsumed { get; set; }
    public double TotalCaloriesBurned { get; set; }
    public DateOnly LogDate { get; init; }
}
