using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.GymUp.Api.Controllers.Common.Controllers;
using ZeroFat.GymUp.Application.Creator.PlanWishlists;


namespace ZeroFat.GymUp.Api.Controllers.Creator;

internal sealed class PlanWishlistsController : BaseController
{
    private readonly IWorkoutModule _workoutModule;

    public PlanWishlistsController(IWorkoutModule workoutModule)
    {
        _workoutModule = workoutModule;
    }

    [HttpPost("search")]
    public async Task<PaginationResponse<PlanWishlistSimplifyDto>> SearchAsync(SearchPlanWishlistsRequest request)
        => await _workoutModule.ExecuteQueryAsync(request);

    [HttpPost]
    public async Task<Result> CreateAsync(CreateRemovePlanWishlistRequest request)
        => await _workoutModule.ExecuteCommandAsync(request);
}
