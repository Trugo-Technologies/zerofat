using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.ClientPortal.Api.Controllers;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Application.Statistics;


namespace ZeroFat.ClientPortal.Api;

internal sealed class StatisticsController : BaseController
{
    private readonly IClientPortalModule _clientModule;

    public StatisticsController(IClientPortalModule clientModule)
    {
        _clientModule = clientModule;
    }


    [HttpGet]
    public async Task<Result<StatisticsDto>> GetAsync()
        => await _clientModule.ExecuteQueryAsync(new GetStatisticsRequest());
}
