using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Catalog.PlanGoals;
public class PlanGoalsBySearchRequestSpec : EntitiesByPaginationFilterSpec<PlanGoal, PlanGoalDto>
{
    public PlanGoalsBySearchRequestSpec(SearchPlanGoalsRequest request)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
             .Where(x => x.IsActive == request.IsActive, request.IsActive.HasValue);
    }
}
public class MobilePlanGoalsBySearchRequestSpec : EntitiesByBaseFilterSpec<PlanGoal, PlanGoalSimplifyDto>
{
    public MobilePlanGoalsBySearchRequestSpec(SearchMobilePlanGoalsRequest request)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn).Where(x => x.IsActive);
    }
}

public class PlanGoalByIdSpec<T> : Specification<PlanGoal, T>
{
    public PlanGoalByIdSpec(DefaultIdType id)
    {
        Query.Where(p => p.Id == id);
    }
}

