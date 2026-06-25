using ZeroFat.Application.Common.Models;
using ZeroFat.Users.Application.Contracts;
using ZeroFat.Users.Application.Roles;
using Microsoft.AspNetCore.Mvc;

namespace ZeroFat.Users.Api.Controllers;
internal sealed class RolesController : BaseController
{
    private readonly IUserModule _userModule;
    public RolesController(IUserModule userModule) => _userModule = userModule;

    [HttpPost]
    public async Task<ActionResult<Result<string>>> CreateAsync(CreateRoleRequest request)
    {
        return await _userModule.ExecuteCommandAsync(request);
    }

    [HttpPost("search")]
    public async Task<ActionResult<PaginationResponse<RoleDto>>> SearchAsync(SearchRolesRequest request)
    {
        return await _userModule.ExecuteQueryAsync(request);
    }

    [HttpPost("assign-roles-user")]
    public async Task<ActionResult<Result>> AssignUsersAsync(AssignUsersToRoleRequest request)
    {
        return await _userModule.ExecuteCommandAsync(request);
    }
}
