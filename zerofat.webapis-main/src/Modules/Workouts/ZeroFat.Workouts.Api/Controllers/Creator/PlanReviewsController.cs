using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.GymUp.Api.Controllers.Common.Controllers;
using ZeroFat.GymUp.Application.Creator.PlanReviews;

namespace ZeroFat.GymUp.Api.Controllers.Creator;

internal sealed class PlanReviewsController : BaseController
{
    private readonly IWorkoutModule _workoutModule;

    public PlanReviewsController(IWorkoutModule workoutModule)
    {
        _workoutModule = workoutModule;
    }

    [AllowAnonymous]
    [HttpPost("search")]
    public async Task<PaginationResponse<PlanReviewSimplifyDto>> SearchAsync(SearchPlanReviewsRequest request)
        => await _workoutModule.ExecuteQueryAsync(request);

    [HttpPost]
    public async Task<Result<DefaultIdType>> CreateAsync(UpsertPlanReviewRequest request)
        => await _workoutModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(DefaultIdType id)
       => await _workoutModule.ExecuteCommandAsync(new DeletePlanReviewRequest(id));


}
