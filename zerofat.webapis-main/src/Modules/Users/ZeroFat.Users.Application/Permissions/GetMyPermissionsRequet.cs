using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Validation;

namespace ZeroFat.Users.Application.Permissions;
public sealed class GetMyPermissionsRequet : IQuery<Result<List<string>>>
{
}

public class GetMyPermissionsRequetValidator : CustomValidator<GetMyPermissionsRequet>
{
    public GetMyPermissionsRequetValidator() // IStringLocalizer<GetMyPermissionsRequetValidator> localizer
    {

    }
}


public class GetMyPermissionsRequetHandler(
    UserManager<ApplicationUser> applicationUserManager,
    RoleManager<ApplicationRole> applicationRoleManager,
    ICurrentUser currentUser) : IQueryHandler<GetMyPermissionsRequet, Result<List<string>>>
{
    private readonly UserManager<ApplicationUser> _applicationUserManager = applicationUserManager;
    private readonly RoleManager<ApplicationRole> _applicationRoleManager = applicationRoleManager;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<Result<List<string>>> Handle(GetMyPermissionsRequet request, CancellationToken cancellationToken)
    {
        var permissions = new List<string>();
        var user = await _applicationUserManager.Users.FirstOrDefaultAsync(x => x.PublicId == _currentUser.GetUserId(), cancellationToken);
        _ = user ?? throw new NotFoundException("User not found");
        var userRoles = await _applicationUserManager.GetRolesAsync(user);
        foreach (var role in await _applicationRoleManager.Roles.Where(r => userRoles.Contains(r.Name)).ToListAsync(cancellationToken))
        {
            var roleClaims = await _applicationRoleManager.GetClaimsAsync(role);
            permissions.AddRange(roleClaims.Select(x => x.Value));
        }

        return await Result<List<string>>.SuccessAsync(data: permissions);
    }
}
