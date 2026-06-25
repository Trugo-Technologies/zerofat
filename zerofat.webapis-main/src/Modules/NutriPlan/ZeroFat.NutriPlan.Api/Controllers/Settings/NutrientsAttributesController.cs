using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.NutriPlan.Application.NutrientsAttributes;
using ZeroFat.NutriPlan.Application.Contracts;
using ZeroFat.NutriPlan.Api.Controllers.Common.Controllers;

namespace ZeroFat.NutriPlan.Api.Controllers.Settings;

internal sealed class NutrientsAttributesController : BaseController
{
    private readonly INutriPlanModule _nutriPlanModule;

    public NutrientsAttributesController(INutriPlanModule nutriPlanModule) => _nutriPlanModule = nutriPlanModule;

    [HttpPost("search")]
    public async Task<PaginationResponse<NutrientsAttributeDto>> SearchAsync(SearchNutrientsAttributesRequest request)
        => await _nutriPlanModule.ExecuteQueryAsync(request);

    [HttpPost]
    public async Task<Result> CreateAsync(CreateNutrientsAttributeRequest request)
        => await _nutriPlanModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(Guid id)
       => await _nutriPlanModule.ExecuteCommandAsync(new DeleteNutrientsAttributeRequest(id));

    [HttpGet("{id:guid}")]
    public async Task<Result<NutrientsAttributeDetailsDto>> GetAsync(Guid id)
       => await _nutriPlanModule.ExecuteQueryAsync(new GetNutrientsAttributeRequest(id));

}

