using ZeroFat.Application.Common.Interfaces;
using ZeroFat.GymUp.Application.Catalog.BodyParts;
using ZeroFat.GymUp.Application.Creator.Workouts;

namespace ZeroFat.GymUp.Application.Creator.WorkoutBodyParts;

public class WorkoutBodyPartSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType WorkoutId { get; set; }
    public DefaultIdType BodyPartId { get; set; }
    public WorkoutSimplifyDto? Workout { get; set; }
    public BodyPartSimplifyDto? BodyPart { get; set; }
}
