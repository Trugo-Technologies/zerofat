using Microsoft.AspNetCore.Mvc;

namespace ZeroFat.Users.Api.Controllers;

internal sealed class DevicesController : BaseController
{
    // private readonly IUserModule _userModule;
    // 
    // public UsersController(IUserModule userModule) => _userModule = userModule;

    [HttpGet("GetAllUsers")]
    public async Task<ActionResult> GetAll()
    {
        // var usersList = await _userModule.ExecuteQueryAsync(new GetAllUsersQuery());
        // 
        // return new GetAllUsersResponse
        // {
        //     UserListVm = usersList
        // };

        return Ok();
    }
}

