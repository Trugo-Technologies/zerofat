using Mapster;
using MediatR;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Enums;

namespace ZeroFat.GymUp.Application.Creator.Plans;
public class SearchMobilePlansRequest : PaginationFilter, IQuery<PaginationResponse<PlanMobileDto>>
{
    public DefaultIdType? PlanGoalId { get; set; }
    public bool? IsWishlist { get; set; }
    public GymEnvironment? Environment { get; set; }
}

public class SearchMobilePlansRequestHandler(IReadRepository<Plan> repository, ICurrentUser currentUser) : IRequestHandler<SearchMobilePlansRequest, PaginationResponse<PlanMobileDto>>
{
    private readonly IReadRepository<Plan> _repository = repository;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<PaginationResponse<PlanMobileDto>> Handle(SearchMobilePlansRequest request, CancellationToken cancellationToken)
    {
        TypeAdapterConfig config = new();
        config.NewConfig<Plan, PlanMobileDto>()
                .Map(destination => destination.TotalRate, src => src.PlanReviews.Sum(x => x.TotalRate))
                .Map(destination => destination.IsWishlist, src => src.PlanWishlists.Any(x => x.UserId == _currentUser.GetUserId()));

        return await _repository.PaginatedListAsync(new MobilePlansBySearchRequestSpec(request, _currentUser.GetUserId()), request.PageNumber, request.PageSize, config, cancellationToken);

    }
}
