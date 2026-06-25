using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.GymUp.Api.Controllers.Common.Controllers;
using ZeroFat.GymUp.Application.Creator.ExerciseBodyParts;


namespace ZeroFat.GymUp.Api.Controllers.Creator;

internal sealed class ExerciseBodyPartsController : BaseController
{
    private readonly IWorkoutModule _workoutModule;

    public ExerciseBodyPartsController(IWorkoutModule workoutModule)
    {
        _workoutModule = workoutModule;
    }

    [HttpPost("search")]
    public async Task<PaginationResponse<ExerciseBodyPartSimplifyDto>> SearchAsync(SearchExerciseBodyPartsRequest request)
        => await _workoutModule.ExecuteQueryAsync(request);

    [HttpPost]
    public async Task<Result> CreateAsync(CreateExerciseBodyPartRequest request)
        => await _workoutModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(Guid id)
       => await _workoutModule.ExecuteCommandAsync(new DeleteExerciseBodyPartRequest(id));
}
