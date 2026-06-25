using Microsoft.AspNetCore.Mvc;

namespace ZeroFat.Api.Controllers;

[Route("api/core-module/v{version:apiVersion}/[controller]")]
internal abstract class VersionedBaseController : BaseController
{
}
