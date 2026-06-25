
using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Creator.PlanReviews;
public class PlanReviewBySearchRequestSpec : EntitiesByPaginationFilterSpec<PlanReview, PlanReviewSimplifyDto>
{
    public PlanReviewBySearchRequestSpec(SearchPlanReviewsRequest request)
        : base(request)
    {
        Query.Where(x => x.UserId == request.UserId, request.UserId.HasValue)
             .Where(x => x.PlanId == request.PlanId, request.PlanId.HasValue)
             .OrderByDescending(c => c.CreatedOn, !request.HasOrderBy());
    }
}

public class PlanReviewByIdRequestSpecByIdSpec<T> : Specification<PlanReview, T>
{
    public PlanReviewByIdRequestSpecByIdSpec(DefaultIdType id) => Query.Where(p => p.Id == id);
}
