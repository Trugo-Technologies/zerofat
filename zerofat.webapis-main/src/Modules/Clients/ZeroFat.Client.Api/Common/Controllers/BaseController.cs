using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZeroFat.ClientPortal.Domain;

namespace ZeroFat.ClientPortal.Api.Controllers;


[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/clientPortal-module/[controller]")]
[ApiExplorerSettings(GroupName = ModuleConstant.ModuleName)]
internal abstract class BaseController : ControllerBase
{
    protected UnauthorizedObjectResult UnauthorizedWithReason(string message)
    {
        return new UnauthorizedObjectResult("");
    }
}
