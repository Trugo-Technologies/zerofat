using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.GymUp.Api.Controllers.Common.Controllers;
using ZeroFat.GymUp.Application.Creator.Exercises;
using ZeroFat.GymUp.Application.Creator.Trainers;


namespace ZeroFat.GymUp.Api.Controllers.Creator;

internal sealed class ExercisesController : BaseController
{
    private readonly IWorkoutModule _workoutModule;

    public ExercisesController(IWorkoutModule workoutModule)
    {
        _workoutModule = workoutModule;
    }

    [HttpPost("search")]
    public async Task<PaginationResponse<ExerciseDto>> SearchAsync(SearchExercisesRequest request)
        => await _workoutModule.ExecuteQueryAsync(request);

    [HttpPost]
    public async Task<Result> CreateAsync([FromForm] CreateExerciseRequest request)
        => await _workoutModule.ExecuteCommandAsync(request);

    [HttpGet("{id:guid}")]
    public async Task<Result<ExerciseDetailsDto>> GetAsync(DefaultIdType id)
        => await _workoutModule.ExecuteQueryAsync(new GetExerciseRequest(id));

    [HttpPut("{id}")]
    public async Task<ActionResult<Result>> UpdateAsync([FromForm] UpdateExerciseRequest request, DefaultIdType id)
       => request.Id != id ? BadRequest() :
            await _workoutModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(DefaultIdType id)
       => await _workoutModule.ExecuteCommandAsync(new DeleteExerciseRequest(id));

    [HttpPut("active/{id}")]
    public async Task<Result> ToggleActiveAsync(DefaultIdType id)
       => await _workoutModule.ExecuteCommandAsync(new ActiveExerciseRequest(id));

    [HttpDelete]
    public async Task<ActionResult<Result>> DeleteAsync(DeleteExercisesRequest request)
    => await _workoutModule.ExecuteCommandAsync(request);

    [HttpPut("active")]
    public async Task<Result> ToggleActivesAsync(ActiveExercisesRequest request)
       => await _workoutModule.ExecuteCommandAsync(request);

}
