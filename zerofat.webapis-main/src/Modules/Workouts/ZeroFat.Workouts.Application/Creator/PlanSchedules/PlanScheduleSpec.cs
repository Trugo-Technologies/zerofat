using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Creator.PlanSchedules;
public class PlanSchedulesBySearchRequestSpec : EntitiesByPaginationFilterSpec<PlanSchedule, PlanScheduleSimplifyDto>
{
    public PlanSchedulesBySearchRequestSpec(SearchPlanSchedulesRequest request)
        : base(request)
    {
        Query.OrderBy(c => c.Day, !request.HasOrderBy()).ThenBy(x => x.Index, !request.HasOrderBy())
             .Where(x => x.PlanId == request.PlanId, request.PlanId.HasValue)
             .Where(x => x.WorkoutId == request.WorkoutId, request.WorkoutId.HasValue)
             .Where(x => x.Daytime == request.Daytime, request.Daytime.HasValue);
    }
}

public class PlanScheduleByIdSpec<T> : Specification<PlanSchedule, T>
{
    public PlanScheduleByIdSpec(DefaultIdType id)
    {
        Query.Where(p => p.Id == id);
    }
}

public class PlanScheduleByPlanIdSpec<T> : Specification<PlanSchedule, T>
{
    public PlanScheduleByPlanIdSpec(DefaultIdType planId)
    {
        Query.Where(p => p.PlanId == planId)
            .OrderBy(x => x.Day);
    }
}

