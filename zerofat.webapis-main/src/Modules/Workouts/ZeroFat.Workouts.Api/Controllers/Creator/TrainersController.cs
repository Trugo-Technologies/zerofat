using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.GymUp.Api.Controllers.Common.Controllers;
using ZeroFat.GymUp.Application.Creator.Trainers;


namespace ZeroFat.GymUp.Api.Controllers.Creator;

internal sealed class TrainersController : BaseController
{
    private readonly IWorkoutModule _workoutModule;

    public TrainersController(IWorkoutModule workoutModule)
    {
        _workoutModule = workoutModule;
    }

    [HttpPost("search")]
    public async Task<PaginationResponse<TrainerDto>> SearchAsync(SearchTrainersRequest request)
        => await _workoutModule.ExecuteQueryAsync(request);

    [HttpPost]
    public async Task<Result> CreateAsync([FromForm] CreateTrainerRequest request)
        => await _workoutModule.ExecuteCommandAsync(request);

    [HttpGet("{id:guid}")]
    public async Task<Result<TrainerDetailsDto>> GetAsync(Guid id)
        => await _workoutModule.ExecuteQueryAsync(new GetTrainerRequest(id));

    [HttpPut("{id}")]
    public async Task<ActionResult<Result>> UpdateAsync([FromForm] UpdateTrainerRequest request, Guid id)
       => request.Id != id ? BadRequest() :
            await _workoutModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(Guid id)
       => await _workoutModule.ExecuteCommandAsync(new DeleteTrainerRequest(id));

    [HttpPut("active/{id}")]
    public async Task<Result> ToggleActiveAsync(Guid id)
       => await _workoutModule.ExecuteCommandAsync(new ActiveTrainerRequest(id));

    [HttpDelete]
    public async Task<ActionResult<Result>> DeleteAsync(DeleteTrainersRequest request)
        => await _workoutModule.ExecuteCommandAsync(request);

    [HttpPut("active")]
    public async Task<Result> ToggleActivesAsync(ActiveTrainersRequest request)
       => await _workoutModule.ExecuteCommandAsync(request);
}
