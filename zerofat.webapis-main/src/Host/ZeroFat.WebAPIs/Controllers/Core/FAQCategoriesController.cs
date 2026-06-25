using Microsoft.AspNetCore.Mvc;
using ZeroFat.Api.Controllers;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Core.Contracts;
using ZeroFat.Application.Core.FAQCategories;

namespace ZeroFat.WebAPIs.Controllers.Core;

internal sealed class FaqCategoriesController : BaseController
{
    private readonly ICoreModule _sharedModule;

    public FaqCategoriesController(ICoreModule sharedModule)
    {
        _sharedModule = sharedModule;
    }

    [HttpPost("search")]
    public async Task<PaginationResponse<FaqCategoryDto>> SearchAsync(SearchFaqCategoriesRequest request)
        => await _sharedModule.ExecuteQueryAsync(request);

    [HttpPost]
    public async Task<Result> CreateAsync(CreateFaqCategoryRequest request)
        => await _sharedModule.ExecuteCommandAsync(request);

    [HttpPut("{id}")]
    public async Task<ActionResult<Result>> UpdateAsync(UpdateFaqCategoryRequest request, DefaultIdType id)
       => request.Id != id ? BadRequest() :
            await _sharedModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(DefaultIdType id)
       => await _sharedModule.ExecuteCommandAsync(new DeleteFaqCategoryRequest(id));

    [HttpPut("active/{id}")]
    public async Task<Result> ToggleActiveAsync(DefaultIdType id)
       => await _sharedModule.ExecuteCommandAsync(new ActiveFaqCategoryRequest(id));

    [HttpGet("{id:guid}")]
    public async Task<Result<FaqCategoryDetailsDto>> GetAsync(DefaultIdType id)
       => await _sharedModule.ExecuteQueryAsync(new GetFaqCategoryRequest(id));

}
