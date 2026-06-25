using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Creator.Plans;
public class PlansBySearchRequestSpec : EntitiesByPaginationFilterSpec<Plan, PlanDto>
{
    public PlansBySearchRequestSpec(SearchPlansRequest request)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
             .Where(x => x.Level == request.Level, request.Level.HasValue)
             .Where(x => x.TrainerId == request.TrainerId, request.TrainerId.HasValue)
             .Where(x => x.PlanGoalId == request.PlanGoalId, request.PlanGoalId.HasValue)
             .Where(x => x.Environment == request.Environment, request.Environment.HasValue)
             .Where(x => x.IsActive == request.IsActive, request.IsActive.HasValue);
    }
}

public class MobilePlansBySearchRequestSpec : EntitiesByPaginationFilterSpec<Plan, PlanMobileDto>
{
    public MobilePlansBySearchRequestSpec(SearchMobilePlansRequest request, Guid? userId)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
             .Where(x => x.PlanGoalId == request.PlanGoalId, request.PlanGoalId.HasValue)
             .Where(x => x.Environment == request.Environment, request.Environment.HasValue)
             .Where(x => x.PlanWishlists.Any(x => x.UserId == userId), request.IsWishlist.HasValue && userId.HasValue)
             .Where(x => x.IsActive);
    }
}

public class PlanByIdSpec<T> : Specification<Plan, T>
{
    public PlanByIdSpec(DefaultIdType id) => Query.Where(p => p.Id == id);
}

