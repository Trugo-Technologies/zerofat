using ZeroFat.ClientPortal.Domain.NutritionTracking;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Domain.ClientManagement;

public class Client : ActivationEntity, IAggregateRoot
{
    public string? StripeId { get; set; }
    public string? FullName { get; set; }
    public string? ImageUrl { get; set; }
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public Gender? Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public List<Guid> ClientAllergicIds { get; set; } = [];

    public double? HeightInCM { get; set; }
    public double? WeightInKG { get; set; }
    public double? CurrentWeightInKG { get; set; }
    public double? TargetWeightInKG { get; set; }
    public DietitianGoal DietitianGoal { get; set; }
    public double ActivityValue { get; set; }

    public Guid? ClientSubscriptionId { get; set; }
    public DateOnly? NewGoalStart { get; set; }
    public SubscriptionStatus SubscriptionStatus { get; set; }

    public double BMI { get; set; }
    public double BMR { get; set; }
    public double BodyFat { get; set; }
    public double TDEE { get; set; }
    public int TimeToReachGoalInDays { get; set; }
    public double NeededCaloriesToReachGoal { get; set; }
    public bool AccountIsDeleted { get; set; }

    public virtual ICollection<ClientLocation> Locations { get; set; } = new HashSet<ClientLocation>();
    public virtual ICollection<ClientGoal> Goals { get; set; } = new HashSet<ClientGoal>();
}
