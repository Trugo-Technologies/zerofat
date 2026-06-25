using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Creator.PlanWishlists;
public class PlanWishlistsBySearchRequestSpec : EntitiesByPaginationFilterSpec<PlanWishlist, PlanWishlistSimplifyDto>
{
    public PlanWishlistsBySearchRequestSpec(SearchPlanWishlistsRequest request)
        : base(request)
    {
        Query.Where(x => x.PlanId == request.PlanId, request.PlanId.HasValue)
             .Where(x => x.UserId == request.UserId, request.UserId.HasValue);
    }
}

public class PlanWishlistByIdSpec<T> : Specification<PlanWishlist, T>
{
    public PlanWishlistByIdSpec(DefaultIdType id)
    {
        Query.Where(p => p.Id == id);
    }
}

