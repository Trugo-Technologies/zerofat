using ZeroFat.Application.Common.Interfaces;
using ZeroFat.GymUp.Application.Creator.Exercises;

namespace ZeroFat.GymUp.Application.Creator.WorkoutExercises;
public class WorkoutExerciseSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType WorkoutId { get; set; }
    public DefaultIdType ExerciseId { get; set; }
    public int Index { get; set; }
    public int? SetIndex { get; set; }
    public int? Reps { get; set; }
    public int? Weight { get; set; }
    public string? SetNameAr { get; set; }
    public string? SetNameEn { get; set; }

    public int? DurationInSec { get; set; }
    public int? Sets { get; set; }
    public ExerciseSimplifyDto? Exercise { get; set; }
}
