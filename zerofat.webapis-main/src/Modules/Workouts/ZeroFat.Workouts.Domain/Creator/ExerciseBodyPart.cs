using ZeroFat.GymUp.Domain.Catalog;

namespace ZeroFat.GymUp.Domain.Creator;

public class ExerciseBodyPart : Entity, IAggregateRoot
{
    public DefaultIdType ExerciseId { get; set; }
    public DefaultIdType BodyPartId { get; set; }
    public virtual Exercise? Exercise { get; set; }
    public virtual BodyPart? BodyPart { get; set; }
}
