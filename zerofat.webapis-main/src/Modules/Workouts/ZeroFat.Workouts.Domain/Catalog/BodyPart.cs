using ZeroFat.GymUp.Domain.Creator;

namespace ZeroFat.GymUp.Domain.Catalog;

public class BodyPart : ActivationEntity, IAggregateRoot
{
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;

    public virtual ICollection<WorkoutBodyPart> Workouts { get;  }
    public BodyPart()
    {
        Workouts = new HashSet<WorkoutBodyPart>();
    }
}
