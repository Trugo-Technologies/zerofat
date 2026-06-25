using MediatR;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.ClientPortal.Domain.NutritionTracking;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.ClientManagement.DailyStatistics;
public class SearchClientDailyStatisticsRequest : PaginationFilter, IQuery<PaginationResponse<ClientDailyStatisticsSimplifyDto>>
{
    public DefaultIdType? ClientId { get; set; }
    public StatisticsType? StatisticsType { get; set; }

}


public class SearchClientDailyStatisticsRequestHandler(IReadRepository<ClientDailyStatistics> repository) : IRequestHandler<SearchClientDailyStatisticsRequest, PaginationResponse<ClientDailyStatisticsSimplifyDto>>
{
    private readonly IReadRepository<ClientDailyStatistics> _repository = repository;

    public async Task<PaginationResponse<ClientDailyStatisticsSimplifyDto>> Handle(SearchClientDailyStatisticsRequest request, CancellationToken cancellationToken)
         => await _repository.PaginatedListAsync(new ClientDailyStatisticsBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);

}
