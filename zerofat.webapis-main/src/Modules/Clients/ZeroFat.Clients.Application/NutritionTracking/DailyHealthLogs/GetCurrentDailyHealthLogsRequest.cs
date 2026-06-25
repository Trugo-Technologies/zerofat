using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.ClientPortal.Domain.NutritionTracking;

namespace ZeroFat.ClientPortal.Application.NutritionTracking.DailyHealthLogs;

public class GetCurrentDailyHealthLogRequest : IQuery<Result<DailyHealthLogRawDto>>
{
    public GetCurrentDailyHealthLogRequest(DateOnly date)
    {
        Date = date;
    }

    public DateOnly Date { get; set; }
}

public class GetCurrentDailyHealthLogRequestHandler(
    IRepositoryWithEvents<DailyHealthLog> repository,
    ICurrentUser currentUser,
    IStringLocalizer<GetCurrentDailyHealthLogRequestHandler> localizer) : IRequestHandler<GetCurrentDailyHealthLogRequest, Result<DailyHealthLogRawDto>>
{

    public async Task<Result<DailyHealthLogRawDto>> Handle(GetCurrentDailyHealthLogRequest request, CancellationToken cancellationToken)
    {
        var entity = await repository.FirstOrDefaultAsync(new CurrentDailyHealthLogSpec<DailyHealthLogRawDto>(currentUser.GetUserId(), request.Date), cancellationToken);
        return await Result<DailyHealthLogRawDto>.SuccessAsync(data: entity);
    }
}
