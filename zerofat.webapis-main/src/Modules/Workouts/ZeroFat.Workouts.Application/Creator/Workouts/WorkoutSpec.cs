using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Creator.Workouts;

public class WorkoutsBySearchRequestSpec : EntitiesByPaginationFilterSpec<Workout, WorkoutDto>
{
    public WorkoutsBySearchRequestSpec(SearchWorkoutsRequest request)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
             .Where(x => x.Level == request.Level, request.Level.HasValue)
             .Where(x => x.TrainerId == request.TrainerId, request.TrainerId.HasValue)
             .Where(x => x.Format == request.Format, request.Format.HasValue)
             .Where(x => x.WorkoutTypeId == request.WorkoutTypeId, request.WorkoutTypeId.HasValue)
             .Where(x => x.WorkoutBodyParts.Any(z => z.BodyPartId == request.BodyPartId), request.BodyPartId.HasValue)
             .Where(x => x.WorkoutEquipments.Any(z => z.EquipmentId == request.EquipmentId), request.EquipmentId.HasValue)
             .Where(x => x.IsActive == request.IsActive, request.IsActive.HasValue);
    }
}

public class WorkoutByIdSpec<T> : Specification<Workout, T>
{
    public WorkoutByIdSpec(DefaultIdType id) => Query.Where(p => p.Id == id);
}
