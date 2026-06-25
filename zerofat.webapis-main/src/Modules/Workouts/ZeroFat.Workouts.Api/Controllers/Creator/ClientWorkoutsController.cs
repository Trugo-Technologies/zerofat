using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.GymUp.Api.Controllers.Common.Controllers;
using ZeroFat.GymUp.Application.Creator.ClientWorkouts;


namespace ZeroFat.GymUp.Api.Controllers.Creator;

internal sealed class ClientWorkoutsController : BaseController
{
    private readonly IWorkoutModule _workoutModule;

    public ClientWorkoutsController(IWorkoutModule workoutModule)
    {
        _workoutModule = workoutModule;
    }

    [HttpPost("search")]
    public async Task<PaginationResponse<ClientWorkoutDetailsDto>> SearchAsync(SearchClientWorkoutsRequest request)
        => await _workoutModule.ExecuteQueryAsync(request);

    [HttpPost]
    public async Task<Result> CreateAsync(CreateClientWorkoutRequest request)
        => await _workoutModule.ExecuteCommandAsync(request);
}
