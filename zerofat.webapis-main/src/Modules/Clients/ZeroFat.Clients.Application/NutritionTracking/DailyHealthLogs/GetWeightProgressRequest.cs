using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.NutritionTracking.DailyHealthLogs;

public class GetWeightProgressRequest : IQuery<Result<WeightProgressReport>>
{
    public GetWeightProgressRequest(StatisticsType timeframe)
    {
        Timeframe = timeframe;
    }

    public StatisticsType Timeframe { get; set; }
}

public class GetWeightProgressRequestHandler(
    IWeightProgressService weightService,
    ICurrentUser currentUser,
    IStringLocalizer<GetWeightProgressRequestHandler> localizer) : IRequestHandler<GetWeightProgressRequest, Result<WeightProgressReport>>
{

    public async Task<Result<WeightProgressReport>> Handle(GetWeightProgressRequest request, CancellationToken cancellationToken)
    {
        var report = await weightService.GetWeightProgressReportAsync(request.Timeframe, currentUser.GetUserId());

        return await Result<WeightProgressReport>.SuccessAsync(data: report);
    }
}
