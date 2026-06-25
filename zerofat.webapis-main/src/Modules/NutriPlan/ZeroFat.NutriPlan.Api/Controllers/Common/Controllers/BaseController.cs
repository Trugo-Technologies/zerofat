using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZeroFat.NutriPlan.Domain;

namespace ZeroFat.NutriPlan.Api.Controllers.Common.Controllers;


[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/nutriplan-module/[controller]")]
[ApiExplorerSettings(GroupName = ModuleConstant.ModuleName)]
internal abstract class BaseController : ControllerBase
{
    protected UnauthorizedObjectResult UnauthorizedWithReason(string message)
    {
        return new UnauthorizedObjectResult("");
    }
}
