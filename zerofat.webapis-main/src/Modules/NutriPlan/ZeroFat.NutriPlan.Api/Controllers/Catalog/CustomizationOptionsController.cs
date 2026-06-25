using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.NutriPlan.Api.Controllers.Common.Controllers;
using ZeroFat.NutriPlan.Application.Contracts;
using ZeroFat.NutriPlan.Application.MealPlanning.MealCustomizationOptions;

namespace ZeroFat.NutriPlan.Api.Controllers.Catalog;

internal sealed class CustomizationOptionsController : BaseController
{
    private readonly INutriPlanModule _nutriPlanModule;

    public CustomizationOptionsController(INutriPlanModule nutriPlanModule)
    {
        _nutriPlanModule = nutriPlanModule;
    }

    [HttpPost("search")]
    public async Task<PaginationResponse<MealCustomizationOptionDto>> SearchAsync(SearchMealCustomizationOptionsRequest request)
        => await _nutriPlanModule.ExecuteQueryAsync(request);


    [HttpGet("{id:guid}")]
    public async Task<Result<MealCustomizationOptionDetailsDto>> GetAsync(Guid id)
        => await _nutriPlanModule.ExecuteQueryAsync(new GetMealCustomizationOptionRequest(id));

    [HttpPost]
    public async Task<Result> CreateAsync([FromForm] CreateMealCustomizationOptionRequest request)
        => await _nutriPlanModule.ExecuteCommandAsync(request);

    [HttpPut("{id}")]
    public async Task<ActionResult<Result>> UpdateAsync([FromForm] UpdateMealCustomizationOptionRequest request, Guid id)
       => request.Id != id ? BadRequest() :
            await _nutriPlanModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(Guid id)
       => await _nutriPlanModule.ExecuteCommandAsync(new DeleteMealCustomizationOptionRequest(id));

}
