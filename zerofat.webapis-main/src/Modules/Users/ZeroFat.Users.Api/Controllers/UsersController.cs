using ZeroFat.Application.Common.Models;
using ZeroFat.Users.Application.Contracts;
using ZeroFat.Users.Application.Users;
using ZeroFat.Users.Infrastructure.Auth.Jwt;
using Microsoft.AspNetCore.Mvc;
using ZeroFat.Domain.Enums;
using Microsoft.AspNetCore.Authorization;

namespace ZeroFat.Users.Api.Controllers;
internal sealed class UsersController : BaseController
{
    private readonly IUserModule _userModule;

    public UsersController(IUserModule userModule)
    {
        _userModule = userModule;
    }

    [HttpPost("search")]
    public async Task<ActionResult<PaginationResponse<UserDto>>> SearchAsync(SearchUsersRequest request)
    {
        return await _userModule.ExecuteQueryAsync(request);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<Result<string>>> CreateAsync([FromForm] CreateUserRequest request)
    {
        request.UserType = UserType.Admin;
        return await _userModule.ExecuteCommandAsync(request);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Result<UserDto>>> UpdateAsync(string id)
        => await _userModule.ExecuteCommandAsync(new GetUserRequest(id));


    [HttpPut("{id}")]
    public async Task<ActionResult<Result>> UpdateAsync([FromForm] UpdateUserRequest request, string id)
        => request.Id != id ? BadRequest() :
             await _userModule.ExecuteCommandAsync(request);

    [HttpPut("personal/ChangePhoneOrEmail")]
    public async Task<ActionResult<Result>> UpdateAsync(UpdateUserPhoneOrEmailRequest request)
        => await _userModule.ExecuteCommandAsync(request);

    [HttpDelete("{id}")]
    public async Task<Result<string>> DeleteAsync(string id)
        => await _userModule.ExecuteCommandAsync(new DeleteUserRequest(id));


    [HttpPut("enable-2fa")]
    public async Task<ActionResult<Result>> CreateAsync(Enable2FAUserRequest request)
    {
        return await _userModule.ExecuteCommandAsync(request);
    }

    [NotSameUser]
    [HttpPut("active/{userId}")]
    public async Task<Result> ToggleActiveAsync(string userId)
    {
        return await _userModule.ExecuteCommandAsync(new ActiveUserRequest(userId));
    }

    [NotSameUser]
    [HttpPost("{userId}/assign-roles")]
    public async Task<ActionResult<Result>> AssignRolesAsync(Guid userId, AssignRolesToUserRequest request)
    {
        request.UserId = userId;
        return await _userModule.ExecuteCommandAsync(request);
    }

    [NotSameUser]
    [HttpPost("reset-password")]
    public async Task<Result> ResetPassword([FromBody] ResetUserPasswordRequest request)
    {
        return await _userModule.ExecuteCommandAsync(request);
    }
}
