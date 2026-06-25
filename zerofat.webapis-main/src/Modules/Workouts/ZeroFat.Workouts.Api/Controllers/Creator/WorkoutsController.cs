using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.GymUp.Api.Controllers.Common.Controllers;
using ZeroFat.GymUp.Application.Creator.Trainers;
using ZeroFat.GymUp.Application.Creator.Workouts;


namespace ZeroFat.GymUp.Api.Controllers.Creator;

internal sealed class WorkoutsController : BaseController
{
    private readonly IWorkoutModule _workoutModule;

    public WorkoutsController(IWorkoutModule workoutModule)
    {
        _workoutModule = workoutModule;
    }

    [HttpPost("search")]
    public async Task<PaginationResponse<WorkoutDto>> SearchAsync(SearchWorkoutsRequest request)
        => await _workoutModule.ExecuteQueryAsync(request);

    [HttpPost]
    public async Task<Result> CreateAsync([FromForm] CreateWorkoutRequest request)
        => await _workoutModule.ExecuteCommandAsync(request);

    [HttpGet("{id:guid}")]
    public async Task<Result<WorkoutDetailsDto>> GetAsync(DefaultIdType id)
        => await _workoutModule.ExecuteQueryAsync(new GetWorkoutRequest(id));

    [HttpGet("withExercises/{id:guid}")]
    public async Task<Result<WorkoutMobileDetailsDto>> GetWithExercisesAsync(DefaultIdType id)
        => await _workoutModule.ExecuteQueryAsync(new GetWorkoutWithExercisesRequest(id));

    [HttpPut("{id}")]
    public async Task<ActionResult<Result>> UpdateAsync([FromForm] UpdateWorkoutRequest request, DefaultIdType id)
       => request.Id != id ? BadRequest() :
            await _workoutModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(DefaultIdType id)
       => await _workoutModule.ExecuteCommandAsync(new DeleteWorkoutRequest(id));

    [HttpPut("active/{id}")]
    public async Task<Result> ToggleActiveAsync(DefaultIdType id)
       => await _workoutModule.ExecuteCommandAsync(new ActiveWorkoutRequest(id));

    [HttpDelete]
    public async Task<ActionResult<Result>> DeleteAsync(DeleteWorkoutsRequest request)
        => await _workoutModule.ExecuteCommandAsync(request);

    [HttpPut("active")]
    public async Task<Result> ToggleActivesAsync(ActiveWorkoutsRequest request)
       => await _workoutModule.ExecuteCommandAsync(request);
}
