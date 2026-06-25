using MediatR;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;

namespace ZeroFat.GymUp.Application.Creator.PlanWishlists;
public class SearchPlanWishlistsRequest : PaginationFilter, IQuery<PaginationResponse<PlanWishlistSimplifyDto>>
{
    public DefaultIdType? PlanId { get; set; }
    public DefaultIdType? UserId { get; set; }
}


public class SearchPlanWishlistsRequestHandler(
    IReadRepository<PlanWishlist> repository,
    ICurrentUser currentUser) : IRequestHandler<SearchPlanWishlistsRequest, PaginationResponse<PlanWishlistSimplifyDto>>
{
    private readonly IReadRepository<PlanWishlist> _repository = repository;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<PaginationResponse<PlanWishlistSimplifyDto>> Handle(SearchPlanWishlistsRequest request, CancellationToken cancellationToken)
    {
        return await _repository.PaginatedListAsync(new PlanWishlistsBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);
    }
}
