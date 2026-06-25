using MediatR;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.ClientPortal.Domain.NutritionTracking;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.NutritionTracking.DailyHealthLogs;
public class SearchDailyHealthLogsRequest : PaginationFilter, IQuery<PaginationResponse<DailyHealthLogSimplifyDto>>
{
    public StatisticsType? StatisticsType { get; set; }
}


public class SearchDailyHealthLogsRequestHandler(
    IReadRepository<DailyHealthLog> repository,
    ICurrentUser currentUser) : IRequestHandler<SearchDailyHealthLogsRequest, PaginationResponse<DailyHealthLogSimplifyDto>>
{
    private readonly IReadRepository<DailyHealthLog> _repository = repository;

    public async Task<PaginationResponse<DailyHealthLogSimplifyDto>> Handle(SearchDailyHealthLogsRequest request, CancellationToken cancellationToken)
         => await _repository.PaginatedListAsync(new DailyHealthLogBySearchRequestSpec(request, currentUser.GetUserId()), request.PageNumber, request.PageSize, cancellationToken);

}
