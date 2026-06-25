using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.GymUp.Api.Controllers.Common.Controllers;
using ZeroFat.GymUp.Application.Creator.WorkoutEquipments;


namespace ZeroFat.GymUp.Api.Controllers.Creator;

internal sealed class WorkoutEquipmentsController : BaseController
{
    private readonly IWorkoutModule _workoutModule;

    public WorkoutEquipmentsController(IWorkoutModule workoutModule)
    {
        _workoutModule = workoutModule;
    }

    [HttpPost("search")]
    public async Task<PaginationResponse<WorkoutEquipmentSimplifyDto>> SearchAsync(SearchWorkoutEquipmentsRequest request)
        => await _workoutModule.ExecuteQueryAsync(request);

    [HttpPost]
    public async Task<Result> CreateAsync(CreateWorkoutEquipmentRequest request)
        => await _workoutModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(Guid id)
       => await _workoutModule.ExecuteCommandAsync(new DeleteWorkoutEquipmentRequest(id));
}
