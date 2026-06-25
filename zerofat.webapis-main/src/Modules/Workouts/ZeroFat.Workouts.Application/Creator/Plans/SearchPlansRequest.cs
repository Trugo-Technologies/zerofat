using MediatR;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Enums;

namespace ZeroFat.GymUp.Application.Creator.Plans;
public class SearchPlansRequest : PaginationFilter, IQuery<PaginationResponse<PlanDto>>
{
    public bool? IsActive { get; set; }
    public Level? Level { get; set; }
    public DefaultIdType? TrainerId { get; set; }
    public DefaultIdType? PlanGoalId { get; set; }
    public GymEnvironment? Environment { get; set; }
}

public class SearchPlansRequestHandler(IReadRepository<Plan> repository) : IRequestHandler<SearchPlansRequest, PaginationResponse<PlanDto>>
{
    private readonly IReadRepository<Plan> _repository = repository;

    public async Task<PaginationResponse<PlanDto>> Handle(SearchPlansRequest request, CancellationToken cancellationToken)
         => await _repository.PaginatedListAsync(new PlansBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);

}
