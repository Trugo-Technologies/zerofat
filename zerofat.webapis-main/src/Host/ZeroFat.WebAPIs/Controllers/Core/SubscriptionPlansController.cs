using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Core.Contracts;
using ZeroFat.Application.Core.SubscriptionPlans;

namespace ZeroFat.Api.Controllers.Core;

internal sealed class SubscriptionPlansController : BaseController
{
    private readonly ICoreModule _sharedModule;

    public SubscriptionPlansController(ICoreModule sharedModule) => _sharedModule = sharedModule;

    [HttpPost("search")]
    public async Task<PaginationResponse<SubscriptionPlanSimplifyDto>> SearchAsync(SearchSubscriptionPlansRequest request)
        => await _sharedModule.ExecuteQueryAsync(request);


    [HttpGet("{id:guid}")]
    public async Task<Result<SubscriptionPlanDetailsDto>> GetAsync(Guid id)
        => await _sharedModule.ExecuteQueryAsync(new GetSubscriptionPlanRequest(id));

    [HttpPut("{id}")]
    public async Task<ActionResult<Result>> UpdateAsync(UpdateSubscriptionPlanRequest request, Guid id)
       => request.Id != id ? BadRequest() :
            await _sharedModule.ExecuteCommandAsync(request);


    [HttpPut("active/{id}")]
    public async Task<Result> ToggleActiveAsync(Guid id)
       => await _sharedModule.ExecuteCommandAsync(new ActiveSubscriptionPlanRequest(id));
    
    [HttpPut("Publish/{id}")]
    public async Task<Result> PublishAsync(Guid id)
       => await _sharedModule.ExecuteCommandAsync(new PublishSubscriptionPlanRequest(id));
}
