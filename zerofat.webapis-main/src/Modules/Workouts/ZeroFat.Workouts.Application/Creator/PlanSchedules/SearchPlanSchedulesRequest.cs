using MediatR;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Enums;

namespace ZeroFat.GymUp.Application.Creator.PlanSchedules;
public class SearchPlanSchedulesRequest : PaginationFilter, IQuery<PaginationResponse<PlanScheduleSimplifyDto>>
{
    public Daytime? Daytime { get; set; }
    public bool? IsRestDay { get; set; }
    public DefaultIdType? PlanId { get; set; }
    public DefaultIdType? WorkoutId { get; set; }
}


public class SearchPlanSchedulesRequestHandler(IReadRepository<PlanSchedule> repository) : IRequestHandler<SearchPlanSchedulesRequest, PaginationResponse<PlanScheduleSimplifyDto>>
{
    private readonly IReadRepository<PlanSchedule> _repository = repository;

    public async Task<PaginationResponse<PlanScheduleSimplifyDto>> Handle(SearchPlanSchedulesRequest request, CancellationToken cancellationToken)
         => await _repository.PaginatedListAsync(new PlanSchedulesBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);

}
