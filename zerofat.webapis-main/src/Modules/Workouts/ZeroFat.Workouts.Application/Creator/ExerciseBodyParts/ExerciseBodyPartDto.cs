using ZeroFat.Application.Common.Interfaces;
using ZeroFat.GymUp.Application.Catalog.BodyParts;
using ZeroFat.GymUp.Application.Creator.Exercises;

namespace ZeroFat.GymUp.Application.Creator.ExerciseBodyParts;

public class ExerciseBodyPartSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType ExerciseId { get; set; }
    public DefaultIdType BodyPartId { get; set; }
    public ExerciseSimplifyDto? Exercise { get; set; }
    public BodyPartSimplifyDto? BodyPart { get; set; }
}
