using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.NutriPlan.Api.Controllers.Common.Controllers;
using ZeroFat.NutriPlan.Application.Contracts;
using ZeroFat.NutriPlan.Application.MealPlanning.Extras;

namespace ZeroFat.NutriPlan.Api.Controllers.Catalog;

internal sealed class ExtrasController : BaseController
{
    private readonly INutriPlanModule _nutriPlanModule;

    public ExtrasController(INutriPlanModule nutriPlanModule)
    {
        _nutriPlanModule = nutriPlanModule;
    }

    [HttpPost("search")]
    public async Task<PaginationResponse<ExtraDto>> SearchAsync(SearchExtrasRequest request)
        => await _nutriPlanModule.ExecuteQueryAsync(request);

    [HttpPost("search-mobile")]
    public async Task<Result<List<ExtraIngredientMobileDto>>> SearchAsync(SearchMobileExtrasRequest request)
        => await _nutriPlanModule.ExecuteQueryAsync(request);

    [HttpGet("{id:guid}")]
    public async Task<Result<ExtraDetailsDto>> GetAsync(Guid id)
        => await _nutriPlanModule.ExecuteQueryAsync(new GetExtraRequest(id));

    [HttpPost]
    public async Task<Result> CreateAsync([FromForm] CreateExtraRequest request)
        => await _nutriPlanModule.ExecuteCommandAsync(request);

    [HttpPut("{id}")]
    public async Task<ActionResult<Result>> UpdateAsync([FromForm] UpdateExtraRequest request, Guid id)
       => request.Id != id ? BadRequest() :
            await _nutriPlanModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(Guid id)
       => await _nutriPlanModule.ExecuteCommandAsync(new DeleteExtraRequest(id));

}
