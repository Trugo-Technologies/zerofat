using Microsoft.AspNetCore.Mvc;
using ZeroFat.Application.Common.Models;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Application.Settings.ClientPortalSettings;

namespace ZeroFat.ClientPortal.Api.Controllers;

internal class ClientPortalSettingsController : BaseController
{
    private readonly IClientPortalModule _ClientPortalModule;
    public ClientPortalSettingsController(IClientPortalModule ClientPortalModule) => _ClientPortalModule = ClientPortalModule;

    [HttpGet]
    public async Task<ActionResult<Result<ClientPortalSetting>>> GetAsync()
    {
        var request = new GetSettingRequest();
        return await _ClientPortalModule.ExecuteQueryAsync(request);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateAsync(UpdateSettingRequest request)
    {
        if (request == null)
        {
            return BadRequest();
        }
        return Ok(await _ClientPortalModule.ExecuteCommandAsync(request));
    }

    [HttpPatch]
    public async Task<ActionResult> PatchAsync(PatchSettingRequest request)
    {
        if (request == null)
        {
            return BadRequest();
        }
        return Ok(await _ClientPortalModule.ExecuteCommandAsync(request));
    }
}

