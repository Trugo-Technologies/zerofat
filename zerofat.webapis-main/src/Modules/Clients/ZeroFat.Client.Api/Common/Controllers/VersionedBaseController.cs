using Microsoft.AspNetCore.Mvc;

namespace ZeroFat.ClientPortal.Api.Controllers;

[Route("api/clientPortal-module/v{version:apiVersion}/[controller]")]
internal abstract class VersionedBaseController : BaseController
{
}
