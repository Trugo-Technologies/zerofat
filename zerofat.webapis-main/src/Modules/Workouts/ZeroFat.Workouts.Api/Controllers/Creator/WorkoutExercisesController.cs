using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.GymUp.Api.Controllers.Common.Controllers;
using ZeroFat.GymUp.Application.Creator.WorkoutExercises;


namespace ZeroFat.GymUp.Api.Controllers.Creator;

internal sealed class WorkoutExercisesController : BaseController
{
    private readonly IWorkoutModule _workoutModule;

    public WorkoutExercisesController(IWorkoutModule workoutModule)
    {
        _workoutModule = workoutModule;
    }

    [HttpPost("search")]
    public async Task<PaginationResponse<WorkoutExerciseSimplifyDto>> SearchAsync(SearchWorkoutExercisesRequest request)
        => await _workoutModule.ExecuteQueryAsync(request);

    [HttpPost]
    public async Task<Result> CreateAsync(CreateWorkoutExerciseRequest request)
        => await _workoutModule.ExecuteCommandAsync(request);

    [HttpPut("{id}")]
    public async Task<ActionResult<Result>> UpdateAsync(UpdateWorkoutExerciseRequest request, Guid id)
       => request.Id != id ? BadRequest() :
            await _workoutModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(Guid id)
       => await _workoutModule.ExecuteCommandAsync(new DeleteWorkoutExerciseRequest(id));

}
