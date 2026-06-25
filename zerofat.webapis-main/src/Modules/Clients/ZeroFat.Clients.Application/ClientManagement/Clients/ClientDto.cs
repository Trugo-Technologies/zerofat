using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.ClientManagement.Clients;
public class ClientStatusDto : IDto
{
    public DefaultIdType Id { get; set; }
    public bool IsActive { get; set; }
    public bool AccountIsDeleted { get; set; }
}

public class ClientSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? FullName { get; set; }
    public string? ImageUrl { get; set; }
    public string? Mobile { get; set; }
    public Gender? Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public bool IsActive { get; set; }

    public string? Email { get; set; }
    public DefaultIdType? ClientSubscriptionId { get; set; }
    public SubscriptionStatus SubscriptionStatus { get; set; }
}

public class ClientRawDto : ClientSimplifyDto
{
    public double? HeightInCM { get; set; }
    public double? WeightInKG { get; set; }
    public double? CurrentWeightInKG { get; set; }
    public double? TargetWeightInKG { get; set; }
    public DietitianGoal DietitianGoal { get; set; }
    public double ActivityValue { get; set; }

    public double BMI { get; set; }
    public double BMR { get; set; }
    public double BodyFat { get; set; }
    public double TDEE { get; set; }
    public int TimeToReachGoalInDays { get; set; }
    public double NeededCaloriesToReachGoal { get; set; }
}

public class ClientAuditableDto : ClientRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class ClientDto : ClientAuditableDto
{
    public List<DefaultIdType> ClientAllergicIds { get; set; } = new List<DefaultIdType>();
    public List<ClientAllergen> ClientAllergics { get; set; } = new List<ClientAllergen>();
}

public class ClientDetailsDto : BaseEntityActivationDetailsDto
{
    public DefaultIdType Id { get; set; }

    public string? FullName { get; set; }
    public string? ImageUrl { get; set; }
    public string? Mobile { get; set; }
    public Gender? Gender { get; set; }
    public DateTime? BirthDate { get; set; }

    public double? HeightInCM { get; set; }
    public double? WeightInKG { get; set; }
    public double? CurrentWeightInKG { get; set; }
    public double? TargetWeightInKG { get; set; }
    public DietitianGoal DietitianGoal { get; set; }
    public double ActivityValue { get; set; }


    public double BMI { get; set; }
    public double BMR { get; set; }
    public double BodyFat { get; set; }
    public double TDEE { get; set; }
    public int TimeToReachGoalInDays { get; set; }
    public double NeededCaloriesToReachGoal { get; set; }

    public string? Email { get; set; }
    public DefaultIdType? ClientSubscriptionId { get; set; }
    public SubscriptionStatus SubscriptionStatus { get; set; }

    public List<DefaultIdType> ClientAllergicIds { get; set; } = new List<DefaultIdType>();
}


public class ClientAllergen
{
    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public string? IconUrl { get; set; }
}
