using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.GymUp.Api.Controllers.Common.Controllers;
using ZeroFat.GymUp.Application.Catalog.EquipmentCategories;
using ZeroFat.GymUp.Application.Catalog.Equipments;

namespace ZeroFat.GymUp.Api.Controllers.Catalog;

internal sealed class EquipmentsController : BaseController
{
    private readonly IWorkoutModule _workoutModule;

    public EquipmentsController(IWorkoutModule workoutModule)
    {
        _workoutModule = workoutModule;
    }

    [HttpPost("search")]
    public async Task<PaginationResponse<EquipmentDto>> SearchAsync(SearchEquipmentsRequest request)
        => await _workoutModule.ExecuteQueryAsync(request);

    [HttpGet("{id:guid}")]
    public async Task<Result<EquipmentDetailsDto>> GetAsync(DefaultIdType id)
    => await _workoutModule.ExecuteQueryAsync(new GetEquipmentRequest(id));


    [HttpPost]
    public async Task<Result> CreateAsync([FromForm] CreateEquipmentRequest request)
        => await _workoutModule.ExecuteCommandAsync(request);

    [HttpPut("{id}")]
    public async Task<ActionResult<Result>> UpdateAsync([FromForm] UpdateEquipmentRequest request, DefaultIdType id)
       => request.Id != id ? BadRequest() :
            await _workoutModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(DefaultIdType id)
       => await _workoutModule.ExecuteCommandAsync(new DeleteEquipmentRequest(id));

    [HttpPut("active/{id}")]
    public async Task<Result> ToggleActiveAsync(DefaultIdType id)
       => await _workoutModule.ExecuteCommandAsync(new ActiveEquipmentRequest(id));

    [HttpDelete]
    public async Task<ActionResult<Result>> DeleteAsync(DeleteEquipmentsRequest request)
     => await _workoutModule.ExecuteCommandAsync(request);

    [HttpPut("active")]
    public async Task<Result> ToggleActivesAsync(ActiveEquipmentsRequest request)
       => await _workoutModule.ExecuteCommandAsync(request);

}
