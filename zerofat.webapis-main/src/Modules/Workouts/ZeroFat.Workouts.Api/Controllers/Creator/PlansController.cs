using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.GymUp.Api.Controllers.Common.Controllers;
using ZeroFat.GymUp.Application.Creator.Plans;


namespace ZeroFat.GymUp.Api.Controllers.Creator;

internal sealed class PlansController : BaseController
{
    private readonly IWorkoutModule _workoutModule;

    public PlansController(IWorkoutModule workoutModule)
    {
        _workoutModule = workoutModule;
    }

    [HttpPost("search-mobile")]
    public async Task<PaginationResponse<PlanMobileDto>> SearchAsync(SearchMobilePlansRequest request)
        => await _workoutModule.ExecuteQueryAsync(request);

    [HttpPost("search")]
    public async Task<PaginationResponse<PlanDto>> SearchAsync(SearchPlansRequest request)
        => await _workoutModule.ExecuteQueryAsync(request);

    [HttpPost]
    public async Task<Result> CreateAsync([FromForm] CreatePlanRequest request)
        => await _workoutModule.ExecuteCommandAsync(request);

    [HttpGet("{id:guid}")]
    public async Task<Result<PlanDetailsDto>> GetAsync(Guid id)
        => await _workoutModule.ExecuteQueryAsync(new GetPlanRequest(id));

    [HttpGet("withSchedules/{id:guid}")]
    public async Task<Result<PlanMobileDetailsDto>> GetwWthScheduleAsync(Guid id)
        => await _workoutModule.ExecuteQueryAsync(new GetMobilePlanRequest(id));

    [HttpPut("{id}")]
    public async Task<ActionResult<Result>> UpdateAsync([FromForm] UpdatePlanRequest request, Guid id)
       => request.Id != id ? BadRequest() :
            await _workoutModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(Guid id)
       => await _workoutModule.ExecuteCommandAsync(new DeletePlanRequest(id));

    [HttpPut("active/{id}")]
    public async Task<Result> ToggleActiveAsync(Guid id)
       => await _workoutModule.ExecuteCommandAsync(new ActivePlanRequest(id));

    [HttpDelete]
    public async Task<ActionResult<Result>> DeleteAsync(DeletePlansRequest request)
    => await _workoutModule.ExecuteCommandAsync(request);

    [HttpPut("active")]
    public async Task<Result> ToggleActivesAsync(ActivePlansRequest request)
       => await _workoutModule.ExecuteCommandAsync(request);
}
