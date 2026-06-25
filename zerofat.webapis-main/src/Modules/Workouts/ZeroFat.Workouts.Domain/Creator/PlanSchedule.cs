using ZeroFat.Domain.Enums;

namespace ZeroFat.GymUp.Domain.Creator;

public class PlanSchedule : AuditableEntity, IAggregateRoot
{
    public int Day { get; set; }
    public int Index { get; set; }
    public Daytime? Daytime { get; set; }

    public Guid PlanId { get; set; }
    public virtual Plan Plan { get; set; } = default!;
    public Guid? WorkoutId { get; set; }
    public virtual Workout? Workout { get; set; }
}
