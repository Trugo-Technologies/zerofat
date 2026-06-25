using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.ClientPortal.Domain.NutritionTracking;

namespace ZeroFat.ClientPortal.Application.ClientManagement.DailyStatistics;

public class GetClientDailyStatisticsRequest(DefaultIdType id) : IQuery<Result<ClientDailyStatisticsDetailsDto>>
{
    public DefaultIdType Id { get; set; } = id;
}

public class GetClientDailyStatisticsRequestHandler(IRepositoryWithEvents<ClientDailyStatistics> repository, IStringLocalizer<GetClientDailyStatisticsRequestHandler> localizer) : IRequestHandler<GetClientDailyStatisticsRequest, Result<ClientDailyStatisticsDetailsDto>>
{
    private readonly IRepositoryWithEvents<ClientDailyStatistics> _repository = repository;
    private readonly IStringLocalizer<GetClientDailyStatisticsRequestHandler> _localizer = localizer;

    public async Task<Result<ClientDailyStatisticsDetailsDto>> Handle(GetClientDailyStatisticsRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new ClientDailyStatisticsByIdSpec<ClientDailyStatisticsDetailsDto>(request.Id), cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["Statisitcs not found", request.Id]);

        return await Result<ClientDailyStatisticsDetailsDto>.SuccessAsync(entity);
    }
}
