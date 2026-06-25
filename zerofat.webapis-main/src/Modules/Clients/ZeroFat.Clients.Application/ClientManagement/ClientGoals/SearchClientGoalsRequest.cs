using MediatR;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.ClientPortal.Domain.NutritionTracking;

namespace ZeroFat.ClientPortal.Application.ClientManagement.ClientGoals;
public class SearchClientGoalsRequest : PaginationFilter, IQuery<PaginationResponse<ClientGoalSimplifyDto>>
{
    public DefaultIdType? ClientId { get; set; }

}


public class SearchClientGoalsRequestHandler(IReadRepository<ClientGoal> repository) : IRequestHandler<SearchClientGoalsRequest, PaginationResponse<ClientGoalSimplifyDto>>
{
    private readonly IReadRepository<ClientGoal> _repository = repository;

    public async Task<PaginationResponse<ClientGoalSimplifyDto>> Handle(SearchClientGoalsRequest request, CancellationToken cancellationToken)
         => await _repository.PaginatedListAsync(new ClientGoalsBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);

}
