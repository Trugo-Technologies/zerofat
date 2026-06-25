using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Catalog.WorkoutTypes;
public class WorkoutTypesBySearchRequestSpec : EntitiesByPaginationFilterSpec<WorkoutType, WorkoutTypeDto>
{
    public WorkoutTypesBySearchRequestSpec(SearchWorkoutTypesRequest request)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
             .Where(x => x.IsActive == request.IsActive, request.IsActive.HasValue);
    }
}

public class WorkoutTypeByIdSpec<T> : Specification<WorkoutType, T>
{
    public WorkoutTypeByIdSpec(DefaultIdType id)
    {
        Query.Where(p => p.Id == id);
    }
}

