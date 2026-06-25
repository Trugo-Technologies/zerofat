using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.NutriPlan.Api.Controllers.Common.Controllers;
using ZeroFat.NutriPlan.Application.Contracts;
using ZeroFat.NutriPlan.Application.MealPlanning.MealCustomizationGroups;

namespace ZeroFat.NutriPlan.Api.Controllers.Catalog;

internal sealed class CustomizationGroupsController : BaseController
{
    private readonly INutriPlanModule _nutriPlanModule;

    public CustomizationGroupsController(INutriPlanModule nutriPlanModule)
    {
        _nutriPlanModule = nutriPlanModule;
    }

    [HttpPost("search-mobile")]
    public async Task<Result<List<MealCustomizationGroupMobileDto>>> SearchAsync(SearchMobileMealCustomizationGroupsRequest request)
        => await _nutriPlanModule.ExecuteQueryAsync(request);

    [HttpPost("search")]
    public async Task<PaginationResponse<MealCustomizationGroupDto>> SearchAsync(SearchMealCustomizationGroupsRequest request)
        => await _nutriPlanModule.ExecuteQueryAsync(request);


    [HttpGet("{id:guid}")]
    public async Task<Result<MealCustomizationGroupDetailsDto>> GetAsync(Guid id)
        => await _nutriPlanModule.ExecuteQueryAsync(new GetMealCustomizationGroupRequest(id));

    [HttpPost]
    public async Task<Result> CreateAsync([FromForm] CreateMealCustomizationGroupRequest request)
        => await _nutriPlanModule.ExecuteCommandAsync(request);

    [HttpPut("{id}")]
    public async Task<ActionResult<Result>> UpdateAsync([FromForm] UpdateMealCustomizationGroupRequest request, Guid id)
       => request.Id != id ? BadRequest() :
            await _nutriPlanModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(Guid id)
       => await _nutriPlanModule.ExecuteCommandAsync(new DeleteMealCustomizationGroupRequest(id));

}
