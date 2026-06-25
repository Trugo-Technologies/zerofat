using ZeroFat.GymUp.Domain.Catalog;

namespace ZeroFat.GymUp.Domain.Creator;

public class WorkoutEquipment : Entity, IAggregateRoot
{
    public DefaultIdType WorkoutId { get; set; }
    public DefaultIdType EquipmentId { get; set; }
    public virtual Workout? Workout { get; set; }
    public virtual Equipment? Equipment { get; set; }
}
