using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.ClientPortal.Domain.NutritionTracking;

namespace ZeroFat.ClientPortal.Application.ClientManagement.DailyStatistics;

public class GetCurrentWeekClientDailyStatisticsRequest() : IQuery<Result<ClientDailyStatisticsSimplifyDto>>
{
}

public class GetCurrentWeekClientDailyStatisticsRequestHandler(
    IRepositoryWithEvents<ClientDailyStatistics> repository,
    ICurrentUser currentUser,
    IStringLocalizer<GetCurrentWeekClientDailyStatisticsRequestHandler> localizer) : IRequestHandler<GetCurrentWeekClientDailyStatisticsRequest, Result<ClientDailyStatisticsSimplifyDto>>
{
    private readonly IRepositoryWithEvents<ClientDailyStatistics> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<Result<ClientDailyStatisticsSimplifyDto>> Handle(GetCurrentWeekClientDailyStatisticsRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new CurrentWeekClientDailyStatisticsSpec<ClientDailyStatisticsSimplifyDto>(_currentUser.GetUserId()), cancellationToken);
        return await Result<ClientDailyStatisticsSimplifyDto>.SuccessAsync(entity);
    }
}
