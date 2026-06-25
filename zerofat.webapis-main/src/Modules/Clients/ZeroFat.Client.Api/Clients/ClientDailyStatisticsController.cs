using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.ClientPortal.Api.Controllers;
using ZeroFat.ClientPortal.Application.ClientManagement.DailyStatistics;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Application.NutritionTracking.DailyHealthLogs;

namespace ZeroFat.ClientPortal.Api.Clients;

internal sealed class ClientDailyStatisticsController : BaseController
{
    private readonly IClientPortalModule _clientModule;

    public ClientDailyStatisticsController(IClientPortalModule clientModule)
    {
        _clientModule = clientModule;
    }

    [HttpPost("search")]
    public async Task<PaginationResponse<ClientDailyStatisticsSimplifyDto>> SearchAsync(SearchClientDailyStatisticsRequest request)
        => await _clientModule.ExecuteQueryAsync(request);

    [HttpPost]
    public async Task<Result> CreateAsync(CreateClientDailyStatisticsRequest request)
        => await _clientModule.ExecuteCommandAsync(request);

    [HttpGet("{id:guid}")]
    public async Task<Result<ClientDailyStatisticsDetailsDto>> GetAsync(DefaultIdType id)
        => await _clientModule.ExecuteQueryAsync(new GetClientDailyStatisticsRequest(id));

    [HttpGet("current")]
    public async Task<Result<ClientDailyStatisticsDto>> GetCurrentAsync()
        => await _clientModule.ExecuteQueryAsync(new GetCurrentClientDailyStatisticsRequest());

    [HttpGet("current-week")]
    public async Task<Result<ClientDailyStatisticsSimplifyDto>> GetCurrentWeekAsync()
        => await _clientModule.ExecuteQueryAsync(new GetCurrentWeekClientDailyStatisticsRequest());


    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(DefaultIdType id)
       => await _clientModule.ExecuteCommandAsync(new DeleteClientDailyStatisticsRequest(id));

}
