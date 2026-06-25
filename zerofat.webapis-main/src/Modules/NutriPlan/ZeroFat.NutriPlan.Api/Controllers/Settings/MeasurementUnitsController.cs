using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.NutriPlan.Application.MeasurementUnits;
using ZeroFat.NutriPlan.Application.Contracts;
using ZeroFat.NutriPlan.Application.Settings.MeasurementUnits;
using ZeroFat.NutriPlan.Api.Controllers.Common.Controllers;

namespace ZeroFat.NutriPlan.Api.Controllers.Settings;

internal sealed class MeasurementUnitsController : BaseController
{
    private readonly INutriPlanModule _nutriPlanModule;

    public MeasurementUnitsController(INutriPlanModule nutriPlanModule) => _nutriPlanModule = nutriPlanModule;

    [HttpPost("search")]
    public async Task<PaginationResponse<MeasurementUnitDto>> SearchAsync(SearchMeasurementUnitsRequest request)
        => await _nutriPlanModule.ExecuteQueryAsync(request);

    [HttpPost]
    public async Task<Result> CreateAsync([FromForm]CreateMeasurementUnitRequest request)
        => await _nutriPlanModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(Guid id)
       => await _nutriPlanModule.ExecuteCommandAsync(new DeleteMeasurementUnitRequest(id));

    [HttpGet("{id:guid}")]
    public async Task<Result<MeasurementUnitDetailsDto>> GetAsync(Guid id)
       => await _nutriPlanModule.ExecuteQueryAsync(new GetMeasurementUnitRequest(id));

}
