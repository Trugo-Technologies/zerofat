using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.ClientPortal.Api.Controllers;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Application.Discounts.DiscountRules;

namespace ZeroFat.ClientPortal.Api.Discounts;
internal sealed class DiscountRulesController : BaseController
{
    private readonly IClientPortalModule _clientModule;

    public DiscountRulesController(IClientPortalModule clientPortalModule)
    {
        _clientModule = clientPortalModule;
    }

    [HttpPost("search")]
    public async Task<PaginationResponse<DiscountRuleDto>> SearchAsync(SearchDiscountRulesRequest request)
        => await _clientModule.ExecuteQueryAsync(request);

    [HttpPost]
    public async Task<Result> CreateAsync(CreateDiscountRuleRequest request)
        => await _clientModule.ExecuteCommandAsync(request);

    [HttpGet("{id:guid}")]
    public async Task<Result<DiscountRuleDetailsDto>> GetAsync(DefaultIdType id)
        => await _clientModule.ExecuteQueryAsync(new GetDiscountRuleRequest(id));

    [HttpPut("{id}")]
    public async Task<ActionResult<Result>> UpdateAsync(UpdateDiscountRuleRequest request, DefaultIdType id)
       => request.Id != id ? BadRequest() :
            await _clientModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(DefaultIdType id)
       => await _clientModule.ExecuteCommandAsync(new DeleteDiscountRuleRequest(id));

    [HttpPut("active/{id}")]
    public async Task<Result> ToggleActiveAsync(DefaultIdType id)
       => await _clientModule.ExecuteCommandAsync(new ActiveDiscountRuleRequest(id));

    [HttpPost("publish/{id}")]
    public async Task<Result> PublishAsync(DefaultIdType id)
   => await _clientModule.ExecuteCommandAsync(new PublishDiscountRuleRequest(id));

    [HttpDelete]
    public async Task<ActionResult<Result>> DeleteAsync(DeleteDiscountRulesRequest request)
    => await _clientModule.ExecuteCommandAsync(request);

    [HttpPut("active")]
    public async Task<Result> ToggleActivesAsync(ActiveDiscountRulesRequest request)
       => await _clientModule.ExecuteCommandAsync(request);
}
