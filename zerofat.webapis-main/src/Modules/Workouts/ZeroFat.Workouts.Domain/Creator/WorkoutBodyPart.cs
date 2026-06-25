using ZeroFat.GymUp.Domain.Catalog;

namespace ZeroFat.GymUp.Domain.Creator;

public class WorkoutBodyPart : Entity, IAggregateRoot
{
    public DefaultIdType WorkoutId { get; set; }
    public DefaultIdType BodyPartId { get; set; }
    public virtual Workout? Workout { get; set; }
    public virtual BodyPart? BodyPart { get; set; }
}
