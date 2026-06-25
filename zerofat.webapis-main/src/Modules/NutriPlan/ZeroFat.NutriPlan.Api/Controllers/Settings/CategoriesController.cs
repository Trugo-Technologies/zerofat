using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.NutriPlan.Api.Controllers.Common.Controllers;
using ZeroFat.NutriPlan.Application.Contracts;
using ZeroFat.NutriPlan.Application.Settings.Categories;

namespace ZeroFat.NutriPlan.Api.Controllers.Settings;

internal sealed class CategoriesController : BaseController
{
    private readonly INutriPlanModule _nutriPlanModule;

    public CategoriesController(INutriPlanModule nutriPlanModule) => _nutriPlanModule = nutriPlanModule;

    [HttpPost("search")]
    public async Task<PaginationResponse<CategoryDto>> SearchAsync(SearchCategoriesRequest request)
        => await _nutriPlanModule.ExecuteQueryAsync(request);

    [HttpPost]
    public async Task<Result> CreateAsync([FromForm] CreateCategoryRequest request)
        => await _nutriPlanModule.ExecuteCommandAsync(request);

    [HttpPut("{id}")]
    public async Task<ActionResult<Result>> UpdateAsync([FromForm] UpdateCategoryRequest request, DefaultIdType id)
       => request.Id != id ? BadRequest() :
            await _nutriPlanModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(DefaultIdType id)
       => await _nutriPlanModule.ExecuteCommandAsync(new DeleteCategoryRequest(id));

    [HttpGet("{id:guid}")]
    public async Task<Result<CategoryDetailsDto>> GetAsync(DefaultIdType id)
     => await _nutriPlanModule.ExecuteQueryAsync(new GetCategoryRequest(id));

}
