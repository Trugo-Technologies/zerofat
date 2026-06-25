using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.GymUp.Api.Controllers.Common.Controllers;
using ZeroFat.GymUp.Application.Catalog.BodyParts;

namespace ZeroFat.GymUp.Api.Controllers.Catalog;

internal sealed class BodyPartsController : BaseController
{
    private readonly IWorkoutModule _workoutModule;

    public BodyPartsController(IWorkoutModule workoutModule)
    {
        _workoutModule = workoutModule;
    }

    [HttpPost("search")]
    public async Task<PaginationResponse<BodyPartDto>> SearchAsync(SearchBodyPartsRequest request)
        => await _workoutModule.ExecuteQueryAsync(request);

    [HttpGet("{id:guid}")]
    public async Task<Result<BodyPartDetailsDto>> GetAsync(Guid id)
     => await _workoutModule.ExecuteQueryAsync(new GetBodyPartRequest(id));


    [HttpPost]
    public async Task<Result> CreateAsync(CreateBodyPartRequest request)
        => await _workoutModule.ExecuteCommandAsync(request);

    [HttpPut("{id}")]
    public async Task<ActionResult<Result>> UpdateAsync(UpdateBodyPartRequest request, Guid id)
       => request.Id != id ? BadRequest() :
            await _workoutModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(Guid id)
       => await _workoutModule.ExecuteCommandAsync(new DeleteBodyPartRequest(id));

    [HttpPut("active/{id}")]
    public async Task<Result> ToggleActiveAsync(Guid id)
       => await _workoutModule.ExecuteCommandAsync(new ActiveBodyPartRequest(id));

    [HttpDelete]
    public async Task<ActionResult<Result>> DeleteAsync(DeleteBodyPartsRequest request)
     => await _workoutModule.ExecuteCommandAsync(request);

    [HttpPut("active")]
    public async Task<Result> ToggleActivesAsync(ActiveBodyPartsRequest request)
       => await _workoutModule.ExecuteCommandAsync(request);

}
