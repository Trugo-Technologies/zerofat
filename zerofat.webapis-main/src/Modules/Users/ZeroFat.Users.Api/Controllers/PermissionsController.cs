using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ZeroFat.Application.Common.Extensions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Security;
using ZeroFat.Users.Application.Contracts;

namespace ZeroFat.Users.Api.Controllers;

internal sealed class PermissionsController : BaseController
{
    private readonly IUserModule _userModule;
    private readonly IServiceProvider _serviceProvider;
    // private readonly IServiceCollection _serviceDescriptors;

    public PermissionsController(IUserModule userModule, IServiceProvider serviceProvider) // , IServiceCollection serviceDescriptors
    {
        _userModule = userModule;
        _serviceProvider = serviceProvider;
        // _serviceDescriptors = serviceDescriptors;
    }

    [AllowAnonymous]
    [HttpGet("permissions")]
    public async Task<ActionResult<List<InnovatePermission>>> GetPermissionsAsync([FromQuery] string? module, [FromQuery] string? subModule)
    {
        List<InnovatePermission> innovatePermissions = new List<InnovatePermission>();
        var permissionProviders = _serviceProvider.GetServices<IPermissionProvider>();
        foreach (var permissionProvider in permissionProviders)
        {
            if (module.HasValue() && module!.Equals(permissionProvider.Module, StringComparison.OrdinalIgnoreCase))
                innovatePermissions.AddRange((await permissionProvider.GetPermissionsAsync()).ToList());
            else if (module.IsEmpty())
                innovatePermissions.AddRange((await permissionProvider.GetPermissionsAsync()).ToList());
        }

        if (subModule.HasValue())
            innovatePermissions = innovatePermissions.Where(x => x.SubModule.Equals(subModule, StringComparison.OrdinalIgnoreCase)).ToList();

        return Ok(await Result<List<InnovatePermission>>.SuccessAsync(innovatePermissions));
    }

    [AllowAnonymous]
    [HttpGet("module/permissions")]
    public async Task<ActionResult<Result<List<InnovateModulePermission>>>> GetPermissionsModuleListAsync([FromQuery] string? module, [FromQuery] string? subModule)
    {
        List<InnovatePermission> innovatePermissions = new List<InnovatePermission>();
        var permissionProviders = _serviceProvider.GetServices<IPermissionProvider>();
        foreach (var permissionProvider in permissionProviders)
        {
            if (module.HasValue() && module!.Equals(permissionProvider.Module, StringComparison.OrdinalIgnoreCase))
                innovatePermissions.AddRange((await permissionProvider.GetPermissionsAsync()).ToList());
            else if (module.IsEmpty())
                innovatePermissions.AddRange((await permissionProvider.GetPermissionsAsync()).ToList());
        }

        if (subModule.HasValue())
            innovatePermissions = innovatePermissions.Where(x => x.SubModule.Equals(subModule, StringComparison.OrdinalIgnoreCase)).ToList();

        var result = innovatePermissions.GroupBy(x => x.Module).Select(x => new InnovateModulePermission() { Module = x.Key, Tables = x.GroupBy(x => x.Resource).Select(x => new InnovateTablePermission { Table = x.Key, InnovatePermissions = x.ToList() }).ToList() }).ToList();

        return Ok(await Result<List<InnovateModulePermission>>.SuccessAsync(result));
    }

    [AllowAnonymous]
    [HttpGet("table/permissions")]
    public async Task<ActionResult<Result<List<InnovateTablePermission>>>> GetPermissionsTablesListAsync([FromQuery] string? module, [FromQuery] string? subModule)
    {
        List<InnovatePermission> innovatePermissions = new List<InnovatePermission>();
        var permissionProviders = _serviceProvider.GetServices<IPermissionProvider>();
        foreach (var permissionProvider in permissionProviders)
        {
            if (module.HasValue() && module!.Equals(permissionProvider.Module, StringComparison.OrdinalIgnoreCase))
                innovatePermissions.AddRange((await permissionProvider.GetPermissionsAsync()).ToList());
            else if (module.IsEmpty())
                innovatePermissions.AddRange((await permissionProvider.GetPermissionsAsync()).ToList());
        }

        if (subModule.HasValue())
            innovatePermissions = innovatePermissions.Where(x => x.SubModule.Equals(subModule, StringComparison.OrdinalIgnoreCase)).ToList();

        var result = innovatePermissions.GroupBy(x => new { x.Module, x.Resource })
                                        .Select(x => new InnovateTablePermission() { Table = x.Key.Resource, InnovatePermissions = x.ToList() }).ToList();

        return Ok(await Result<List<InnovateTablePermission>>.SuccessAsync(result));
    }

    [AllowAnonymous]
    [HttpGet("my-permissions")]
    public async Task<ActionResult<Result<List<InnovateTablePermission>>>> GetMyPermissionsListAsync([FromQuery] string? module, [FromQuery] string? subModule)
    {
        List<InnovatePermission> innovatePermissions = [];
        var permissionProviders = _serviceProvider.GetServices<IPermissionProvider>();
        foreach (var permissionProvider in permissionProviders)
        {
            if (module.HasValue() && module!.Equals(permissionProvider.Module, StringComparison.OrdinalIgnoreCase))
                innovatePermissions.AddRange((await permissionProvider.GetPermissionsAsync()).ToList());
            else if (module.IsEmpty())
                innovatePermissions.AddRange((await permissionProvider.GetPermissionsAsync()).ToList());
        }

        if (subModule.HasValue())
            innovatePermissions = innovatePermissions.Where(x => x.SubModule.Equals(subModule, StringComparison.OrdinalIgnoreCase)).ToList();

        var result = innovatePermissions.GroupBy(x => new { x.Module, x.Resource })
                                        .Select(x => new InnovateTablePermission() { Table = x.Key.Resource, InnovatePermissions = x.ToList() }).ToList();

        return Ok(await Result<List<InnovateTablePermission>>.SuccessAsync(result));
    }

}

