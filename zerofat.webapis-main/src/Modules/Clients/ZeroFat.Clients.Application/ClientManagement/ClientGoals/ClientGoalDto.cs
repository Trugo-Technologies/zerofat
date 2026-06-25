using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Core.PhysicalActivityLevels;
using ZeroFat.ClientPortal.Application.ClientManagement.Clients;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.ClientManagement.ClientGoals;

public class ClientGoalSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
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
}

public class ClientGoalRawDto : ClientGoalSimplifyDto
{
    public DefaultIdType ClientId { get; set; }
}

public class ClientGoalAuditableDto : ClientGoalRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class ClientGoalDto : ClientGoalAuditableDto
{
    public ClientSimplifyDto? Client { get; set; }
}

public class ClientGoalDetailsDto : BaseEntityAuditableDetailsDto
{
    public DefaultIdType Id { get; set; }
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
    public ClientSimplifyDto? Client { get; set; }
    public PhysicalActivityLevelSimplifyDto? PhysicalActivityLevel { get; set; }
}

