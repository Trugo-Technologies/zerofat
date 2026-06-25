using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.NutriPlan.Api.Controllers.Common.Controllers;
using ZeroFat.NutriPlan.Application.Contracts;
using ZeroFat.NutriPlan.Application.MenuPlanning.DailyMenus;

namespace ZeroFat.NutriPlan.Api.Controllers.DailyMenuPlanning;

internal sealed class DailyMenusController : BaseController
{
    private readonly INutriPlanModule _nutriPlanModule;

    public DailyMenusController(INutriPlanModule nutriPlanModule) => _nutriPlanModule = nutriPlanModule;

    [HttpPost("search")]
    public async Task<PaginationResponse<DailyMenuDto>> SearchAsync(SearchDailyMenusRequest request)
        => await _nutriPlanModule.ExecuteQueryAsync(request);


    [HttpGet("{id:guid}")]
    public async Task<Result<DailyMenuDetailsDto>> GetAsync(Guid id)
        => await _nutriPlanModule.ExecuteQueryAsync(new GetDailyMenuRequest(id));

}
