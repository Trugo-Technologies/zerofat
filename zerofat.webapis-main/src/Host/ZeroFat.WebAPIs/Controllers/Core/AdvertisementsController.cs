using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Core.Contracts;
using ZeroFat.Application.Core.Advertisements;
using ZeroFat.Api.Controllers;

namespace ZeroFat.WebAPIs.Controllers.Core;

internal sealed class AdvertisementsController : BaseController
{
    private readonly ICoreModule _sharedModule;

    public AdvertisementsController(ICoreModule sharedModule) => _sharedModule = sharedModule;

    [HttpPost("search")]
    [AllowAnonymous]
    public async Task<PaginationResponse<AdvertisementDto>> SearchAsync(SearchAdvertisementsRequest request)
        => await _sharedModule.ExecuteQueryAsync(request);

    [HttpPost]
    public async Task<Result> CreateAsync([FromForm] CreateAdvertisementRequest request)
        => await _sharedModule.ExecuteCommandAsync(request);

    [HttpPut("{id}")]
    public async Task<ActionResult<Result>> UpdateAsync([FromForm] UpdateAdvertisementRequest request, DefaultIdType id)
       => request.Id != id ? BadRequest() :
            await _sharedModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(DefaultIdType id)
       => await _sharedModule.ExecuteCommandAsync(new DeleteAdvertisementRequest(id));

    [HttpGet("{id:guid}")]
    public async Task<Result<AdvertisementDetailsDto>> GetAsync(DefaultIdType id)
     => await _sharedModule.ExecuteQueryAsync(new GetAdvertisementRequest(id));


    [HttpPut("active/{id}")]
    public async Task<Result> ToggleActiveAsync(DefaultIdType id)
       => await _sharedModule.ExecuteCommandAsync(new ActiveAdvertisementRequest(id));
}
