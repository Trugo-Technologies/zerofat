using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZeroFat.Domain;

namespace ZeroFat.Api.Controllers;


[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/core-module/[controller]")]
[ApiExplorerSettings(GroupName = ModuleConstant.ModuleName)]
internal abstract class BaseController : ControllerBase
{
    protected UnauthorizedObjectResult UnauthorizedWithReason(string message)
    {
        return new UnauthorizedObjectResult("");
    }
}
