using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.GymUp.Api.Controllers.Common.Controllers;
using ZeroFat.GymUp.Application.Statistics;


namespace ZeroFat.GymUp.Api;

internal sealed class StatisticsController : BaseController
{
    private readonly IWorkoutModule _workoutModule;

    public StatisticsController(IWorkoutModule workoutModule)
    {
        _workoutModule = workoutModule;
    }

    [HttpGet]
    public async Task<Result<StatisticsDto>> GetAsync()
        => await _workoutModule.ExecuteQueryAsync(new GetStatisticsRequest());
}
