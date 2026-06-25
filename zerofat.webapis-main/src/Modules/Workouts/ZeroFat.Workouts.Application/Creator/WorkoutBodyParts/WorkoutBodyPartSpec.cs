using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Creator.WorkoutBodyParts;
public class WorkoutBodyPartsBySearchRequestSpec : EntitiesByPaginationFilterSpec<WorkoutBodyPart, WorkoutBodyPartSimplifyDto>
{
    public WorkoutBodyPartsBySearchRequestSpec(SearchWorkoutBodyPartsRequest request)
        : base(request)
    {
        Query.Where(x => x.BodyPartId == request.BodyPartId, request.BodyPartId.HasValue)
             .Where(x => x.WorkoutId == request.WorkoutId, request.WorkoutId.HasValue);
    }
}

public class WorkoutBodyPartByIdSpec<T> : Specification<WorkoutBodyPart, T>
{
    public WorkoutBodyPartByIdSpec(DefaultIdType id) => Query.Where(p => p.Id == id);
}

