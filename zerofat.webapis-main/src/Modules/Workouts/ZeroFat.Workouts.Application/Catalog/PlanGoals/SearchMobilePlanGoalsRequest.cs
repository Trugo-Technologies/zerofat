using MediatR;
using ZeroFat.Application.Common.Persistence;

namespace ZeroFat.GymUp.Application.Catalog.PlanGoals;
public class SearchMobilePlanGoalsRequest : BaseFilter, IQuery<Result<List<PlanGoalSimplifyDto>>>
{
    public bool? IsActive { get; set; }
}


public class SearchMobilePlanGoalsRequestHandler(IReadRepository<PlanGoal> repository) : IRequestHandler<SearchMobilePlanGoalsRequest, Result<List<PlanGoalSimplifyDto>>>
{
    private readonly IReadRepository<PlanGoal> _repository = repository;

    public async Task<Result<List<PlanGoalSimplifyDto>>> Handle(SearchMobilePlanGoalsRequest request, CancellationToken cancellationToken)
    {
        var result = await _repository.ListAsync(new MobilePlanGoalsBySearchRequestSpec(request), cancellationToken);
        return await Result<List<PlanGoalSimplifyDto>>.SuccessAsync(result);
    }
}
