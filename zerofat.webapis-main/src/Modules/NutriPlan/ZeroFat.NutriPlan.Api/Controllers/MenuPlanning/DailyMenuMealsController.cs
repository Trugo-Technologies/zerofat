using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.NutriPlan.Api.Controllers.Common.Controllers;
using ZeroFat.NutriPlan.Application.Contracts;
using ZeroFat.NutriPlan.Application.MenuPlanning.DailyMenuMeals;

namespace ZeroFat.NutriPlan.Api.Controllers.MenuPlanning;

internal sealed class DailyMenuMealsController : BaseController
{
    private readonly INutriPlanModule _nutriPlanModule;

    public DailyMenuMealsController(INutriPlanModule nutriPlanModule)
    {
        _nutriPlanModule = nutriPlanModule;
    }

    [HttpPost("search")]
    public async Task<PaginationResponse<DailyMenuMealDto>> SearchAsync(SearchDailyMenuMealsRequest request)
        => await _nutriPlanModule.ExecuteQueryAsync(request);

    [HttpPost("current")]
    public async Task<PaginationResponse<MenuMealDto>> SearchCurrentAsync(SearchCurrentDailyMenuMealsRequest request)
        => await _nutriPlanModule.ExecuteQueryAsync(request);

    [HttpGet("mealsbyPlan")]
    public async Task<Result<List<SelecteabeleMealPlanMenuDto>>> SearchCurrentAsync([FromQuery] GetSelectableDailyMenuMealsRequest request)
        => await _nutriPlanModule.ExecuteQueryAsync(request);

    [HttpPost]
    public async Task<Result> CreateAsync(CreateDailyMenuMealRequest request)
        => await _nutriPlanModule.ExecuteCommandAsync(request);

    [HttpPost("meals")]
    public async Task<Result> CreateAsync(AddMealsToMenuRequest request)
        => await _nutriPlanModule.ExecuteCommandAsync(request);

    [HttpPost("multi-meals")]
    public async Task<Result> CreateAsync(AddMultiMealsToMenuRequest request)
        => await _nutriPlanModule.ExecuteCommandAsync(request);

    [HttpPut("{id}/setDefailt")]
    public async Task<ActionResult<Result>> UpdateAsync(SetDailyMenuMealAsDefaulRequest request, Guid id)
       => request.Id != id ? BadRequest() :
            await _nutriPlanModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(Guid id)
       => await _nutriPlanModule.ExecuteCommandAsync(new DeleteDailyMenuMealRequest(id));


    // [HttpGet("{id:guid}")]
    // public async Task<Result<DailyMenuMealDetailsDto>> GetAsync(Guid id)
    //     => await _nutriPlanModule.ExecuteQueryAsync(new GetDailyMenuMealRequest(id));

}
