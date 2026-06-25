using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Domain.NutritionTracking;

public class ClientGoal : AuditableEntity, IAggregateRoot
{
    public double? HeightInCM { get; set; }
    public double? WeightInKG { get; set; }
    public double? TargetWeightInKG { get; set; }
    public DietitianGoal DietitianGoal { get; set; }


    public HeightMeasurement? HeightMeasurement { get; set; }
    public WeightMeasurement? WeightMeasurement { get; set; }
    public WeightMeasurement? TargetWeightMeasurement { get; set; }

    public double ActivityValue { get; set; }
    public DefaultIdType PhysicalActivityLevelId { get; set; }

    public double BMI { get; set; }
    public double BMR { get; set; }
    public double BodyFat { get; set; }
    public double TDEE { get; set; }
    public int TimeToReachGoalInDays { get; set; }
    public double NeededCaloriesToReachGoal { get; set; }

    public DefaultIdType ClientId { get; set; }
    public virtual Client Client { get; set; } = default!;
}
