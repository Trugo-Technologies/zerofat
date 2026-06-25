using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.GymUp.Api.Controllers.Common.Controllers;
using ZeroFat.GymUp.Application.Catalog.BodyParts;
using ZeroFat.GymUp.Application.Catalog.EquipmentCategories;

namespace ZeroFat.GymUp.Api.Controllers.Catalog;

internal sealed class EquipmentCategoriesController : BaseController
{
    private readonly IWorkoutModule _workoutModule;

    public EquipmentCategoriesController(IWorkoutModule workoutModule)
    {
        _workoutModule = workoutModule;
    }

    [HttpPost("search")]
    public async Task<PaginationResponse<EquipmentCategoryDto>> SearchAsync(SearchEquipmentCategoriesRequest request)
        => await _workoutModule.ExecuteQueryAsync(request);

    [HttpGet("{id:guid}")]
    public async Task<Result<EquipmentCategoryDetailsDto>> GetAsync(Guid id)
        => await _workoutModule.ExecuteQueryAsync(new GetEquipmentCategoryRequest(id));

    [HttpPost]
    public async Task<Result> CreateAsync([FromForm] CreateEquipmentCategoryRequest request)
        => await _workoutModule.ExecuteCommandAsync(request);

    [HttpPut("{id}")]
    public async Task<ActionResult<Result>> UpdateAsync([FromForm] UpdateEquipmentCategoryRequest request, Guid id)
       => request.Id != id ? BadRequest() :
            await _workoutModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(Guid id)
       => await _workoutModule.ExecuteCommandAsync(new DeleteEquipmentCategoryRequest(id));

    [HttpPut("active/{id}")]
    public async Task<Result> ToggleActiveAsync(Guid id)
       => await _workoutModule.ExecuteCommandAsync(new ActiveEquipmentCategoryRequest(id));

    [HttpDelete]
    public async Task<ActionResult<Result>> DeleteAsync(DeleteEquipmentCategoriesRequest request)
     => await _workoutModule.ExecuteCommandAsync(request);

    [HttpPut("active")]
    public async Task<Result> ToggleActivesAsync(ActiveEquipmentCategoriesRequest request)
       => await _workoutModule.ExecuteCommandAsync(request);

}
