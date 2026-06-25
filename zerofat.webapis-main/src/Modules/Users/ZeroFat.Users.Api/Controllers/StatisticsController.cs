using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.Users.Api.Controllers;
using ZeroFat.Users.Application.Contracts;
using ZeroFat.Users.Application.Statistics;


namespace ZeroFat.Users.Api;

internal sealed class StatisticsController : BaseController
{
    private readonly IUserModule _userModule;

    public StatisticsController(IUserModule userModule)
    {
        _userModule = userModule;
    }

    [HttpGet]
    public async Task<Result<StatisticsDto>> GetAsync()
        => await _userModule.ExecuteQueryAsync(new GetStatisticsRequest());
}
