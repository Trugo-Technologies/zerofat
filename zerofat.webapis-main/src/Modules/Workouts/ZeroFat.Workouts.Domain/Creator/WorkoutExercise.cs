namespace ZeroFat.GymUp.Domain.Creator;

public class WorkoutExercise : AuditableEntity, IAggregateRoot
{
    public DefaultIdType WorkoutId { get; set; }
    public DefaultIdType ExerciseId { get; set; }
    public int Index { get; set; }
    public string? SetNameEn { get; set; }
    public string? SetNameAr { get; set; }
    public int? SetIndex { get; set; }
    public int? Reps { get; set; }
    public int? Weight { get; set; }
    public int? DurationInSec { get; set; }
    public int? Sets { get; set; }
    public virtual Workout? Workout { get; set; }
    public virtual Exercise? Exercise { get; set; }
}
