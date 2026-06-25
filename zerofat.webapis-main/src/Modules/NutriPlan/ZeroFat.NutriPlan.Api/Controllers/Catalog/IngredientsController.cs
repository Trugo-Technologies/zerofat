using GraphQL;
using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.NutriPlan.Api.Controllers.Common.Controllers;
using ZeroFat.NutriPlan.Application.Contracts;
using ZeroFat.NutriPlan.Application.MealPlanning.Ingredients;

namespace ZeroFat.NutriPlan.Api.Controllers.Catalog;

internal sealed class IngredientsController : BaseController
{
    private readonly INutriPlanModule _nutriPlanModule;

    public IngredientsController(INutriPlanModule nutriPlanModule)
    {
        _nutriPlanModule = nutriPlanModule;
    }

    [HttpPost("search")]
    public async Task<PaginationResponse<IngredientDto>> SearchAsync(SearchIngredientsRequest request)
        => await _nutriPlanModule.ExecuteQueryAsync(request);


    [HttpGet("{id:guid}")]
    public async Task<Result<IngredientDetailsDto>> GetAsync(Guid id)
        => await _nutriPlanModule.ExecuteQueryAsync(new GetIngredientRequest(id));

    [HttpPost]
    public async Task<Result> CreateAsync([FromForm] CreateIngredientRequest request)
        => await _nutriPlanModule.ExecuteCommandAsync(request);

    [HttpPost("upload")]
    [AllowAnonymous]
    public async Task<Result> CreateAsync([FromForm] UploadIngredientExcelCommand request)
        => await _nutriPlanModule.ExecuteCommandAsync(request);

    [HttpPut("{id}")]
    public async Task<ActionResult<Result>> UpdateAsync([FromForm] UpdateIngredientRequest request, Guid id)
       => request.Id != id ? BadRequest() :
            await _nutriPlanModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(Guid id)
       => await _nutriPlanModule.ExecuteCommandAsync(new DeleteIngredientRequest(id));


    [HttpDelete]
    public async Task<ActionResult<Result>> DeleteAsync(DeleteIngredientsRequest request)
       => await _nutriPlanModule.ExecuteCommandAsync(request);

}
