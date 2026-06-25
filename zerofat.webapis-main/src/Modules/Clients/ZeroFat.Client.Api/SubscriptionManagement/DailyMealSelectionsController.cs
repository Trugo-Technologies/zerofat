using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.ClientPortal.Api.Controllers;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.ClientSubscriptions;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.DailyMealSelections;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;

namespace ZeroFat.ClientPortal.Api.SubscriptionManagement;

internal sealed class DailyMealSelectionsController(IClientPortalModule clientPortalModule) : BaseController
{
    private readonly IClientPortalModule _clientPortalModule = clientPortalModule;

    [HttpPut("daily-meal")]
    public async Task<Result<DefaultIdType>> UpdateAsync(UpdateDailyMealSelectionRequest request)
        => await _clientPortalModule.ExecuteCommandAsync(request);

    [HttpPut("customiz-daily-meal")]
    public async Task<Result<DefaultIdType>> CustomizAsync(CustomizeDailyMealSelectionRequest request)
        => await _clientPortalModule.ExecuteCommandAsync(request);

    [HttpPost("search")]
    public async Task<PaginationResponse<DailyMealSelectionDto>> SearchAsync(SearchDailyMealSelectionsRequest request)
       => await _clientPortalModule.ExecuteQueryAsync(request);

    [HttpGet("{id:guid}")]
    public async Task<Result<DailyMealSelectionDetailsDto>> GetAsync(DefaultIdType id)
    => await _clientPortalModule.ExecuteQueryAsync(new GetDailyMealSelectionRequest(id));
}
