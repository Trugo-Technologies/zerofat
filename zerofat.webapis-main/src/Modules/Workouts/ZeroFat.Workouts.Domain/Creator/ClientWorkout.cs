namespace ZeroFat.GymUp.Domain.Creator;

public class ClientWorkout : AuditableEntity, IAggregateRoot
{
    public DefaultIdType WorkoutId { get; set; }
    public DefaultIdType UserId { get; set; }
    public DateOnly Date { get; set; }
    public double Calories { get; set; }
    public virtual Workout? Workout { get; set; }
}
