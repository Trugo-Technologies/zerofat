using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.NutriPlan.Api.Controllers.Common.Controllers;
using ZeroFat.NutriPlan.Application.Contracts;
using ZeroFat.NutriPlan.Application.MealPlanning.Recipes;
using ZeroFat.NutriPlan.Application.Settings.MealPlans;
using ZeroFat.NutriPlan.Application.Settings.MealTypes;

namespace ZeroFat.NutriPlan.Api.Controllers.Settings;

internal sealed class MealPlansController : BaseController
{
    private readonly INutriPlanModule _nutriPlanModule;

    public MealPlansController(INutriPlanModule nutriPlanModule) => _nutriPlanModule = nutriPlanModule;

    /// <summary>GET list — optional filter: ?isActive=true|false</summary>
    [HttpGet]
    public Task<Result<List<MealPlanDto>>> GetListAsync([FromQuery] bool? isActive)
        => _nutriPlanModule.ExecuteQueryAsync(new GetMealPlansRequest { IsActive = isActive });

    [HttpPost("search")]
    public async Task<PaginationResponse<MealPlanDto>> SearchAsync(SearchMealPlansRequest request)
        => await _nutriPlanModule.ExecuteQueryAsync(request);

    [HttpPost]
    public async Task<Result> CreateAsync([FromForm] CreateMealPlanRequest request)
        => await _nutriPlanModule.ExecuteCommandAsync(request);

    [HttpPut("{id}")]
    public async Task<ActionResult<Result>> UpdateAsync([FromForm] UpdateMealPlanRequest request, DefaultIdType id)
       => request.Id != id ? BadRequest() :
            await _nutriPlanModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(DefaultIdType id)
       => await _nutriPlanModule.ExecuteCommandAsync(new DeleteMealPlanRequest(id));

    [HttpGet("{id:guid}")]
    public async Task<Result<MealPlanDetailsDto>> GetAsync(DefaultIdType id)
       => await _nutriPlanModule.ExecuteQueryAsync(new GetMealPlanRequest(id));

    [HttpPut("active/{id}")]
    public async Task<Result> ToggleActiveAsync(DefaultIdType id)
     => await _nutriPlanModule.ExecuteCommandAsync(new ActiveMealPlanRequest(id));

    [HttpPost("publish/{id}")]
    public async Task<Result> PublishMealPlanAsync(DefaultIdType id)
    => await _nutriPlanModule.ExecuteCommandAsync(new PublishMealPlanRequest(id));

}
