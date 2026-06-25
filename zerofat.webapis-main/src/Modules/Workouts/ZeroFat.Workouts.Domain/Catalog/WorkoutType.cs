using ZeroFat.GymUp.Domain.Creator;

namespace ZeroFat.GymUp.Domain.Catalog;
public class WorkoutType : ActivationEntity, IAggregateRoot
{
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public string? IconUrl { get; set; }

    public virtual ICollection<Workout> Workouts { get; set; }
    public WorkoutType()
    {
        Workouts = new HashSet<Workout>();
    }
}

