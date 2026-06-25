using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.NutriPlan.Api.Controllers.Common.Controllers;
using ZeroFat.NutriPlan.Application.Contracts;
using ZeroFat.NutriPlan.Application.Statistics;

namespace ZeroFat.NutriPlan.Api;

internal sealed class StatisticsController : BaseController
{
    private readonly INutriPlanModule _nutriPlanModule;

    public StatisticsController(INutriPlanModule nutriPlanModule)
    {
        _nutriPlanModule = nutriPlanModule;
    }

    [HttpGet]
    public async Task<Result<StatisticsDto>> GetAsync()
        => await _nutriPlanModule.ExecuteQueryAsync(new GetStatisticsRequest());
}
