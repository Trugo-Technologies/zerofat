using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.GymUp.Api.Controllers.Common.Controllers;
using ZeroFat.GymUp.Application.Creator.WorkoutBodyParts;


namespace ZeroFat.GymUp.Api.Controllers.Creator;

internal sealed class WorkoutBodyPartsController : BaseController
{
    private readonly IWorkoutModule _workoutModule;

    public WorkoutBodyPartsController(IWorkoutModule workoutModule)
    {
        _workoutModule = workoutModule;
    }

    [HttpPost("search")]
    public async Task<PaginationResponse<WorkoutBodyPartSimplifyDto>> SearchAsync(SearchWorkoutBodyPartsRequest request)
        => await _workoutModule.ExecuteQueryAsync(request);

    [HttpPost]
    public async Task<Result> CreateAsync(CreateWorkoutBodyPartRequest request)
        => await _workoutModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(Guid id)
       => await _workoutModule.ExecuteCommandAsync(new DeleteWorkoutBodyPartRequest(id));
}
