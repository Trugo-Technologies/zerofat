using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Enums;
using ZeroFat.GymUp.Application.Creator.Workouts;

namespace ZeroFat.GymUp.Application.Creator.PlanSchedules;

public class PlanScheduleSimplifyDto : IDto
{
    public Guid Id { get; set; }
    public int Day { get; set; }
    public int Index { get; set; }
    public Daytime? Daytime { get; set; }
    public Guid PlanId { get; set; }
    public Guid? WorkoutId { get; set; }
    public WorkoutSimplifyDto? Workout { get; set; }
}
