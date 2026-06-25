using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.ClientPortal.Api.Controllers;
using ZeroFat.ClientPortal.Application.ClientManagement.Clients;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.DailySelections;

namespace ZeroFat.ClientPortal.Api.SubscriptionManagement;

internal sealed class DailySelectionsController(IClientPortalModule clientPortalModule) : BaseController
{
    private readonly IClientPortalModule _clientPortalModule = clientPortalModule;

    [HttpPost("search")]
    public async Task<PaginationResponse<DailySelectionDto>> SearchAsync(SearchDailySelectionsRequest request)
        => await _clientPortalModule.ExecuteQueryAsync(request);

    [HttpGet("byDate")]
    public async Task<Result<DailySelectionDetailsDto>> GetAsync([FromQuery] GetDailySelectionByDateRequest request)
        => await _clientPortalModule.ExecuteQueryAsync(request);

    [HttpPut("daily-info")]
    public async Task<Result<DefaultIdType>> UpdateAsync(UpdateDailySelectionRequest request)
        => await _clientPortalModule.ExecuteCommandAsync(request);

    [HttpGet("{id:guid}")]
    public async Task<Result<DailySelectionDetailsDto>> GetAsync(DefaultIdType id)
        => await _clientPortalModule.ExecuteQueryAsync(new GetDailySelectionRequest(id));


}
