using ZeroFat.Application.Common.Interfaces;
using ZeroFat.GymUp.Application.Creator.Workouts;

namespace ZeroFat.GymUp.Application.Creator.ClientWorkouts;

public class ClientWorkoutSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType WorkoutId { get; set; }
    public DefaultIdType UserId { get; set; }
    public DateOnly Date { get; set; }
    public double Calories { get; set; }
}

public class ClientWorkoutDetailsDto : BaseEntityAuditableDetailsDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType WorkoutId { get; set; }
    public DefaultIdType UserId { get; set; }
    public DateOnly Date { get; set; }
    public double Calories { get; set; }
    public WorkoutSimplifyDto? Workout { get; set; }
}
