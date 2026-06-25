using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.ClientPortal.Domain.Common.ValueObjects;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Domain.NutritionTracking;

public class DailyHealthLog : AuditableEntity, IAggregateRoot
{
    private readonly List<CalorieRecord> _calorieRecords = new();

    // Primary properties
    public DefaultIdType ClientId { get; set; }
    public virtual Client Client { get; set; } = default!;
    public DateOnly LogDate { get; set; }

    // Measurements
    public IReadOnlyCollection<CalorieRecord> CalorieRecords => _calorieRecords.AsReadOnly();

    public BodyMeasurement? Weight { get; set; }
    public WaterIntake WaterConsumption { get; set; } = WaterIntake.Zero;

    // Persisted Computed Properties
    public double TotalCaloriesConsumed { get; set; }
    public double TotalCaloriesBurned { get; set; }

    // Domain Calculated Property (not persisted)
    public double NetCalories => TotalCaloriesConsumed - TotalCaloriesBurned;

    // Behavior Methods
    public void RecordWeight(BodyMeasurement weight) => Weight = weight;

    public void RecordWaterIntake(WaterIntake water) => WaterConsumption = water;

    public void AddCalorieRecord(CalorieRecord record)
    {
        _calorieRecords.Add(record);
    }

    public void RemoveCalorieRecord(DefaultIdType recordId)
    {
        _calorieRecords.RemoveAll(x => x.Id == recordId);
    }
}
