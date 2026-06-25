using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Core.Contracts;
using ZeroFat.Application.Core.FAQs;

namespace ZeroFat.Api.Controllers.Core;

internal sealed class FaqsController : BaseController
{
    private readonly ICoreModule _sharedModule;

    public FaqsController(ICoreModule sharedModule) => _sharedModule = sharedModule;

    [HttpPost("search")]
    [AllowAnonymous]
    public async Task<PaginationResponse<FaqSimplifyDto>> SearchAsync(SearchFaqsRequest request)
        => await _sharedModule.ExecuteQueryAsync(request);

    [HttpPost]
    public async Task<Result> CreateAsync(CreateFaqRequest request)
        => await _sharedModule.ExecuteCommandAsync(request);

    [HttpGet("{id:guid}")]
    public async Task<Result<FaqDetailsDto>> GetAsync(Guid id)
        => await _sharedModule.ExecuteQueryAsync(new GetFaqRequest(id));

    [HttpPut("{id}")]
    public async Task<ActionResult<Result>> UpdateAsync(UpdateFaqRequest request, Guid id)
       => request.Id != id ? BadRequest() :
            await _sharedModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(Guid id)
       => await _sharedModule.ExecuteCommandAsync(new DeleteFaqRequest(id));

    [HttpPut("active/{id}")]
    public async Task<Result> ToggleActiveAsync(Guid id)
       => await _sharedModule.ExecuteCommandAsync(new ActiveFaqRequest(id));
}
