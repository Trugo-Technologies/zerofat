using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.GymUp.Api.Controllers.Common.Controllers;
using ZeroFat.GymUp.Application.Catalog.Equipments;
using ZeroFat.GymUp.Application.Catalog.PlanGoals;

namespace ZeroFat.GymUp.Api.Controllers.Catalog;

internal sealed class PlanGoalsController : BaseController
{
    private readonly IWorkoutModule _workoutModule;

    public PlanGoalsController(IWorkoutModule workoutModule)
    {
        _workoutModule = workoutModule;
    }

    [HttpPost("search")]
    public async Task<PaginationResponse<PlanGoalDto>> SearchAsync(SearchPlanGoalsRequest request)
        => await _workoutModule.ExecuteQueryAsync(request);

    [HttpPost("search-mobile")]
    public async Task<Result<List<PlanGoalSimplifyDto>>> SearchMobileAsync(SearchMobilePlanGoalsRequest request)
        => await _workoutModule.ExecuteQueryAsync(request);

    [HttpGet("{id:guid}")]
    public async Task<Result<PlanGoalDetailsDto>> GetAsync(Guid id)
        => await _workoutModule.ExecuteQueryAsync(new GetPlanGoalRequest(id));


    [HttpPost]
    public async Task<Result> CreateAsync([FromForm] CreatePlanGoalRequest request)
        => await _workoutModule.ExecuteCommandAsync(request);

    [HttpPut("{id}")]
    public async Task<ActionResult<Result>> UpdateAsync([FromForm] UpdatePlanGoalRequest request, Guid id)
       => request.Id != id ? BadRequest() :
            await _workoutModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(Guid id)
       => await _workoutModule.ExecuteCommandAsync(new DeletePlanGoalRequest(id));

    [HttpPut("active/{id}")]
    public async Task<Result> ToggleActiveAsync(Guid id)
       => await _workoutModule.ExecuteCommandAsync(new ActivePlanGoalRequest(id));

    [HttpDelete]
    public async Task<ActionResult<Result>> DeleteAsync(DeletePlanGoalsRequest request)
    => await _workoutModule.ExecuteCommandAsync(request);

    [HttpPut("active")]
    public async Task<Result> ToggleActivesAsync(ActivePlanGoalsRequest request)
       => await _workoutModule.ExecuteCommandAsync(request);

}
