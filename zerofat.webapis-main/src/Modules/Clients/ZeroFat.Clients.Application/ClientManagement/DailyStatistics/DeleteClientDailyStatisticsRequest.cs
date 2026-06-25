using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.ClientPortal.Domain.NutritionTracking;

namespace ZeroFat.ClientPortal.Application.ClientManagement.DailyStatistics;

public class DeleteClientDailyStatisticsRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType Id { get; set; }
    public DeleteClientDailyStatisticsRequest(DefaultIdType id)
    {
        Id = id;
    }
}


public class DeleteClientDailyStatisticsRequestHandler(IRepository<ClientDailyStatistics> repository, IStringLocalizer<DeleteClientDailyStatisticsRequestHandler> localizer) : IRequestHandler<DeleteClientDailyStatisticsRequest, Result<DefaultIdType>>
{
    private readonly IRepository<ClientDailyStatistics> _repository = repository;
    private readonly IStringLocalizer<DeleteClientDailyStatisticsRequestHandler> _localizer = localizer;

    public async Task<Result<DefaultIdType>> Handle(DeleteClientDailyStatisticsRequest request, CancellationToken cancellationToken)
    {
        var statistics = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = statistics ?? throw new NotFoundException(_localizer["Statistics not found"]);

        await _repository.DeleteAsync(statistics, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(statistics.Id);
    }

}
