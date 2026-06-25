using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.GymUp.Api.Controllers.Common.Controllers;
using ZeroFat.GymUp.Application.Catalog.PlanGoals;
using ZeroFat.GymUp.Application.Catalog.WorkoutTypes;

namespace ZeroFat.GymUp.Api.Controllers.Catalog;

internal sealed class WorkoutTypesController : BaseController
{
    private readonly IWorkoutModule _workoutModule;

    public WorkoutTypesController(IWorkoutModule workoutModule)
    {
        _workoutModule = workoutModule;
    }

    [HttpPost("search")]
    public async Task<PaginationResponse<WorkoutTypeDto>> SearchAsync(SearchWorkoutTypesRequest request)
        => await _workoutModule.ExecuteQueryAsync(request);

    [HttpGet("{id:guid}")]
    public async Task<Result<WorkoutTypeDetailsDto>> GetAsync(Guid id)
      => await _workoutModule.ExecuteQueryAsync(new GetWorkoutTypeRequest(id));

    [HttpPost]
    public async Task<Result> CreateAsync([FromForm] CreateWorkoutTypeRequest request)
        => await _workoutModule.ExecuteCommandAsync(request);

    [HttpPut("{id}")]
    public async Task<ActionResult<Result>> UpdateAsync([FromForm] UpdateWorkoutTypeRequest request, Guid id)
       => request.Id != id ? BadRequest() :
            await _workoutModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(Guid id)
       => await _workoutModule.ExecuteCommandAsync(new DeleteWorkoutTypeRequest(id));

    [HttpPut("active/{id}")]
    public async Task<Result> ToggleActiveAsync(Guid id)
       => await _workoutModule.ExecuteCommandAsync(new ActiveWorkoutTypeRequest(id));

    [HttpDelete]
    public async Task<ActionResult<Result>> DeleteAsync(DeleteWorkoutTypesRequest request)
        => await _workoutModule.ExecuteCommandAsync(request);

    [HttpPut("active")]
    public async Task<Result> ToggleActivesAsync(ActiveWorkoutTypesRequest request)
       => await _workoutModule.ExecuteCommandAsync(request);

}
