using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.NutriPlan.Api.Controllers.Common.Controllers;
using ZeroFat.NutriPlan.Application.Contracts;
using ZeroFat.NutriPlan.Application.Settings.Allergens;

namespace ZeroFat.NutriPlan.Api.Controllers.Settings;

internal sealed class AllergensController : BaseController
{
    private readonly INutriPlanModule _nutriPlanModule;

    public AllergensController(INutriPlanModule nutriPlanModule)
    {
        _nutriPlanModule = nutriPlanModule;
    }

    [HttpPost("search")]
    public async Task<PaginationResponse<AllergenDto>> SearchAsync(SearchAllergensRequest request)
        => await _nutriPlanModule.ExecuteQueryAsync(request);

    [HttpPost]
    public async Task<Result> CreateAsync([FromForm] CreateAllergenRequest request)
        => await _nutriPlanModule.ExecuteCommandAsync(request);

    [HttpPut("{id}")]
    public async Task<ActionResult<Result>> UpdateAsync([FromForm] UpdateAllergenRequest request, DefaultIdType id)
       => request.Id != id ? BadRequest() :
            await _nutriPlanModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(DefaultIdType id)
       => await _nutriPlanModule.ExecuteCommandAsync(new DeleteAllergenRequest(id));

    [HttpGet("{id:guid}")]
    public async Task<Result<AllergenDetailsDto>> GetAsync(DefaultIdType id)
     => await _nutriPlanModule.ExecuteQueryAsync(new GetAllergenRequest(id));

}
