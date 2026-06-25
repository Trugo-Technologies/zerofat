using Microsoft.AspNetCore.Mvc;

namespace ZeroFat.GymUp.Api.Controllers.Common.Controllers;

[Route("api/workout-module/v{version:apiVersion}/[controller]")]
internal abstract class VersionedBaseController : BaseController
{
}
