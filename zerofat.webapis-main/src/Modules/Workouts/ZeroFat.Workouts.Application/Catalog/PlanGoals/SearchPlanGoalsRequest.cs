using MediatR;
using ZeroFat.Application.Common.Persistence;

namespace ZeroFat.GymUp.Application.Catalog.PlanGoals;
public class SearchPlanGoalsRequest : PaginationFilter, IQuery<PaginationResponse<PlanGoalDto>>
{
    public bool? IsActive { get; set; }
}


public class SearchPlanGoalsRequestHandler(IReadRepository<PlanGoal> repository) : IRequestHandler<SearchPlanGoalsRequest, PaginationResponse<PlanGoalDto>>
{
    private readonly IReadRepository<PlanGoal> _repository = repository;

    public async Task<PaginationResponse<PlanGoalDto>> Handle(SearchPlanGoalsRequest request, CancellationToken cancellationToken)
         => await _repository.PaginatedListAsync(new PlanGoalsBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);

}
