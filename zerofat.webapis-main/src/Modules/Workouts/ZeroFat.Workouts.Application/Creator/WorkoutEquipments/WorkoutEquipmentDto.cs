using ZeroFat.Application.Common.Interfaces;
using ZeroFat.GymUp.Application.Catalog.Equipments;
using ZeroFat.GymUp.Application.Creator.Workouts;

namespace ZeroFat.GymUp.Application.Creator.WorkoutEquipments;

public class WorkoutEquipmentSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType WorkoutId { get; set; }
    public DefaultIdType EquipmentId { get; set; }
    public WorkoutSimplifyDto? Workout { get; set; }
    public EquipmentSimplifyDto? Equipment { get; set; }
}
