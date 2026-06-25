using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Core.Contracts;
using ZeroFat.Application.Core.Banners;
using ZeroFat.Api.Controllers;

namespace ZeroFat.WebAPIs.Controllers.Core;

internal sealed class BannersController : BaseController
{
    private readonly ICoreModule _sharedModule;

    public BannersController(ICoreModule sharedModule)
    {
        _sharedModule = sharedModule;
    }

    [HttpPost("search")]
    [AllowAnonymous]
    public async Task<PaginationResponse<BannerDto>> SearchAsync(SearchBannersRequest request)
        => await _sharedModule.ExecuteQueryAsync(request);

    [HttpPost]
    public async Task<Result> CreateAsync([FromForm] CreateBannerRequest request)
        => await _sharedModule.ExecuteCommandAsync(request);

    [HttpPut("{id}")]
    public async Task<ActionResult<Result>> UpdateAsync([FromForm] UpdateBannerRequest request, DefaultIdType id)
       => request.Id != id ? BadRequest() :
            await _sharedModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(DefaultIdType id)
       => await _sharedModule.ExecuteCommandAsync(new DeleteBannerRequest(id));

    [HttpGet("{id:guid}")]
    public async Task<Result<BannerDetailsDto>> GetAsync(DefaultIdType id)
     => await _sharedModule.ExecuteQueryAsync(new GetBannerRequest(id));


    [HttpPut("active/{id}")]
    public async Task<Result> ToggleActiveAsync(DefaultIdType id)
       => await _sharedModule.ExecuteCommandAsync(new ActiveBannerRequest(id));
}
