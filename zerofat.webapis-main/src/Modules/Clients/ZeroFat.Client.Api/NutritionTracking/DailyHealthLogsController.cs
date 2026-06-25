using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.ClientPortal.Api.Controllers;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Application.NutritionTracking.DailyHealthLogs;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Api.NutritionTracking;

internal sealed class DailyHealthLogsController : BaseController
{
    private readonly IClientPortalModule _clientModule;

    public DailyHealthLogsController(IClientPortalModule clientModule)
    {
        _clientModule = clientModule;
    }

    [HttpGet("progress/{timeframe}")]
    public async Task<Result<WeightProgressReport>> SearchAsync(StatisticsType timeframe)
        => await _clientModule.ExecuteQueryAsync(new GetWeightProgressRequest(timeframe));

    [HttpPost("water")]
    public async Task<Result> CreateAsync(SetWaterDailyHealthLogRequest request)
        => await _clientModule.ExecuteCommandAsync(request);

    [HttpPost("weight")]
    public async Task<Result> CreateAsync(SetWeightDailyHealthLogRequest request)
        => await _clientModule.ExecuteCommandAsync(request);

    [HttpPost("calories-record")]
    public async Task<Result> CreateAsync(AddCalorieRecordDailyHealthLogRequest request)
       => await _clientModule.ExecuteCommandAsync(request);

    [HttpGet("current")]
    public async Task<Result<DailyHealthLogRawDto>> GetCurrentAsync([FromQuery] DateOnly date)
        => await _clientModule.ExecuteQueryAsync(new GetCurrentDailyHealthLogRequest(date));

}
