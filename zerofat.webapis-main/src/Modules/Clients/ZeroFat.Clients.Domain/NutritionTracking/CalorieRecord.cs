using ZeroFat.ClientPortal.Domain.Common.ValueObjects;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Domain.NutritionTracking;

// CalorieRecord.cs
public class CalorieRecord : Entity
{
    public DefaultIdType DailyHealthLogId { get; init; }
    public string Name { get; init; } = default!;
    public double Calories { get; init; }
    public CalorieRecordType RecordType { get; init; }
    public NutritionFacts? Nutrition { get; init; }
    public DateTime RecordedAt { get; init; }
    public TimeOnly? MealTime { get; init; }
}
