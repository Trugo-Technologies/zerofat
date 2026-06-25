using Microsoft.AspNetCore.Mvc;
using ZeroFat.NutriPlan.Api.Controllers.Common.Controllers;

namespace ZeroFat.NutriPlan.Api.Controllers;

[Route("api/nutriplan-module/v{version:apiVersion}/[controller]")]
internal abstract class VersionedBaseController : BaseController
{
}
