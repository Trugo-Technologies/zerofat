using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.NutriPlan.Api.Controllers.Common.Controllers;
using ZeroFat.NutriPlan.Application.Contracts;
using ZeroFat.NutriPlan.Application.MealPlanning.Recipes;

namespace ZeroFat.NutriPlan.Api.Controllers.Catalog;

internal sealed class RecipesController : BaseController
{
    private readonly INutriPlanModule _nutriPlanModule;

    public RecipesController(INutriPlanModule nutriPlanModule)
    {
        _nutriPlanModule = nutriPlanModule;
    }

    [HttpPost("search")]
    public async Task<PaginationResponse<RecipeDto>> SearchAsync(SearchRecipesRequest request)
        => await _nutriPlanModule.ExecuteQueryAsync(request);


    [HttpGet("{id:guid}")]
    public async Task<Result<RecipeDetailsDto>> GetAsync(Guid id)
        => await _nutriPlanModule.ExecuteQueryAsync(new GetRecipeRequest(id));

    [HttpPost]
    public async Task<Result> CreateAsync(CreateRecipeRequest request)
        => await _nutriPlanModule.ExecuteCommandAsync(request);

    [HttpPut("{id}")]
    public async Task<ActionResult<Result>> UpdateAsync(UpdateRecipeRequest request, Guid id)
       => request.Id != id ? BadRequest() :
            await _nutriPlanModule.ExecuteCommandAsync(request);

    [HttpPost("image")]
    public async Task<ActionResult<Result>> UpdateAsync([FromForm] AddImageToRecipeRequest request, Guid id)
      => request.Id != id ? BadRequest() :
           await _nutriPlanModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(Guid id)
       => await _nutriPlanModule.ExecuteCommandAsync(new DeleteRecipeRequest(id));

}
