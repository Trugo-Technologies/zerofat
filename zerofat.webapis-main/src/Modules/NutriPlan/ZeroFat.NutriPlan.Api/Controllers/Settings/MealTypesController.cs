using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.NutriPlan.Api.Controllers.Common.Controllers;
using ZeroFat.NutriPlan.Application.Contracts;
using ZeroFat.NutriPlan.Application.Settings.MealTypes;

namespace ZeroFat.NutriPlan.Api.Controllers.Settings;

internal sealed class MealTypesController : BaseController
{
    private readonly INutriPlanModule _nutriPlanModule;

    public MealTypesController(INutriPlanModule nutriPlanModule) => _nutriPlanModule = nutriPlanModule;

    [HttpPost("search")]
    public async Task<PaginationResponse<MealTypeDto>> SearchAsync(SearchMealTypesRequest request)
        => await _nutriPlanModule.ExecuteQueryAsync(request);

    [HttpPost]
    public async Task<Result> CreateAsync([FromForm] CreateMealTypeRequest request)
        => await _nutriPlanModule.ExecuteCommandAsync(request);

    [HttpPut("{id}")]
    public async Task<ActionResult<Result>> UpdateAsync([FromForm] UpdateMealTypeRequest request, DefaultIdType id)
       => request.Id != id ? BadRequest() :
            await _nutriPlanModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(DefaultIdType id)
       => await _nutriPlanModule.ExecuteCommandAsync(new DeleteMealTypeRequest(id));

    [HttpPut("active/{id}")]
    public async Task<Result> ToggleActiveAsync(DefaultIdType id)
       => await _nutriPlanModule.ExecuteCommandAsync(new ActiveMealTypeRequest(id));

    [HttpGet("{id:guid}")]
    public async Task<Result<MealTypeDetailsDto>> GetAsync(DefaultIdType id)
    => await _nutriPlanModule.ExecuteQueryAsync(new GetMealTypeRequest(id));

}
