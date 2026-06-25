using Microsoft.AspNetCore.Mvc;

namespace ZeroFat.Users.Api.Controllers;

[Route("api/users-module/v{version:apiVersion}/[controller]")]
internal abstract class VersionedBaseController : BaseController
{
}
