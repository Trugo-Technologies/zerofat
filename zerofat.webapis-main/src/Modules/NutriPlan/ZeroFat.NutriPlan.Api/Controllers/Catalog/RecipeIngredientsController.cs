using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.NutriPlan.Api.Controllers.Common.Controllers;
using ZeroFat.NutriPlan.Application.Contracts;
using ZeroFat.NutriPlan.Application.MealPlanning.RecipeIngredients;

namespace ZeroFat.NutriPlan.Api.Controllers.Catalog;

internal sealed class RecipeIngredientsController : BaseController
{
    private readonly INutriPlanModule _nutriPlanModule;

    public RecipeIngredientsController(INutriPlanModule nutriPlanModule)
    {
        _nutriPlanModule = nutriPlanModule;
    }

    //[HttpPost("search")]
    //public async Task<PaginationResponse<RecipeIngredientSimplifyDto>> SearchAsync(SearchRecipeIngredientsRequest request)
    //    => await _nutriPlanModule.ExecuteQueryAsync(request);

    [HttpPost]
    public async Task<Result> CreateAsync(CreateRecipeIngredientRequest request)
        => await _nutriPlanModule.ExecuteCommandAsync(request);

    [HttpPut("{id}")]
    public async Task<ActionResult<Result>> UpdateAsync(UpdateRecipeIngredientRequest request, Guid id)
       => request.Id != id ? BadRequest() :
            await _nutriPlanModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(Guid id)
       => await _nutriPlanModule.ExecuteCommandAsync(new DeleteRecipeIngredientRequest(id));

}
