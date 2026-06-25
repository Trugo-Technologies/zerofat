using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.NutriPlan.Api.Controllers.Common.Controllers;
using ZeroFat.NutriPlan.Application.Contracts;
using ZeroFat.NutriPlan.Application.MenuPlanning.Menus;

namespace ZeroFat.NutriPlan.Api.Controllers.MenuPlanning;

internal sealed class MenusController : BaseController
{
    private readonly INutriPlanModule _nutriPlanModule;

    public MenusController(INutriPlanModule nutriPlanModule)
    {
        _nutriPlanModule = nutriPlanModule;
    }

    [HttpPost("search")]
    public async Task<PaginationResponse<MenuDto>> SearchAsync(SearchMenusRequest request)
        => await _nutriPlanModule.ExecuteQueryAsync(request);


    [HttpGet("{id:guid}")]
    public async Task<Result<MenuDetailsDto>> GetAsync(Guid id)
        => await _nutriPlanModule.ExecuteQueryAsync(new GetMenuRequest(id));

    [HttpPost]
    public async Task<Result> CreateAsync(CreateMenuRequest request)
        => await _nutriPlanModule.ExecuteCommandAsync(request);

    [HttpPost("copy")]
    public async Task<Result> CreateAsync(CopyMenuRequest request)
        => await _nutriPlanModule.ExecuteCommandAsync(request);

    [HttpPut("{id}/publish")]
    public async Task<ActionResult<Result>> UpdateAsync(PublishMenuRequest request, Guid id)
       => request.Id != id ? BadRequest() :
            await _nutriPlanModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<ActionResult<Result>> DeleteAsync(Guid id)
       => await _nutriPlanModule.ExecuteCommandAsync(new DeleteMenuRequest(id));

}
