using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Creator.WorkoutEquipments;
public class WorkoutEquipmentsBySearchRequestSpec : EntitiesByPaginationFilterSpec<WorkoutEquipment, WorkoutEquipmentSimplifyDto>
{
    public WorkoutEquipmentsBySearchRequestSpec(SearchWorkoutEquipmentsRequest request)
        : base(request)
    {
        Query.Where(x => x.EquipmentId == request.EquipmentId, request.EquipmentId.HasValue)
             .Where(x => x.WorkoutId == request.WorkoutId, request.WorkoutId.HasValue);
    }
}

public class WorkoutEquipmentByIdSpec<T> : Specification<WorkoutEquipment, T>
{
    public WorkoutEquipmentByIdSpec(DefaultIdType id)
    {
        Query.Where(p => p.Id == id);
    }
}

