using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.ClientPortal.Domain.NutritionTracking;

namespace ZeroFat.ClientPortal.Application.ClientManagement.DailyStatistics;

public class GetCurrentClientDailyStatisticsRequest() : IQuery<Result<ClientDailyStatisticsDto>>
{
}

public class GetCurrentClientDailyStatisticsRequestHandler(
    IRepositoryWithEvents<ClientDailyStatistics> repository,
    ICurrentUser currentUser,
    IStringLocalizer<GetCurrentClientDailyStatisticsRequestHandler> localizer) : IRequestHandler<GetCurrentClientDailyStatisticsRequest, Result<ClientDailyStatisticsDto>>
{
    private readonly IRepositoryWithEvents<ClientDailyStatistics> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<Result<ClientDailyStatisticsDto>> Handle(GetCurrentClientDailyStatisticsRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new CurrentClientDailyStatisticsSpec<ClientDailyStatisticsDto>(_currentUser.GetUserId(), DateOnly.FromDateTime(DateTime.Now)), cancellationToken);
        return await Result<ClientDailyStatisticsDto>.SuccessAsync(entity);
    }
}
