using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Security;
using ZeroFat.Application.Common.Validation;

namespace ZeroFat.Users.Application.Permissions;
public sealed class GetMyPermissionsByTableRequet : IQuery<Result<List<SimpleTablePermission>>>
{
}

public class GetMyPermissionsByTableRequetValidator : CustomValidator<GetMyPermissionsByTableRequet>
{
    public GetMyPermissionsByTableRequetValidator() // IStringLocalizer<GetMyPermissionsByTableRequetValidator> localizer
    {

    }
}


public class GetMyPermissionsByTableRequetHandler(
    UserManager<ApplicationUser> applicationUserManager,
    RoleManager<ApplicationRole> applicationRoleManager,
    ICurrentUser currentUser) : IQueryHandler<GetMyPermissionsByTableRequet, Result<List<SimpleTablePermission>>>
{
    private readonly UserManager<ApplicationUser> _applicationUserManager = applicationUserManager;
    private readonly RoleManager<ApplicationRole> _applicationRoleManager = applicationRoleManager;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<Result<List<SimpleTablePermission>>> Handle(GetMyPermissionsByTableRequet request, CancellationToken cancellationToken)
    {
        var permissions = new List<SimpleTablePermission>();
        var user = await _applicationUserManager.Users.FirstOrDefaultAsync(x => x.PublicId == _currentUser.GetUserId(), cancellationToken);
        _ = user ?? throw new NotFoundException("User not found");
        var userRoles = await _applicationUserManager.GetRolesAsync(user);
        foreach (var role in await _applicationRoleManager.Roles.Where(r => userRoles.Contains(r.Name)).ToListAsync(cancellationToken))
        {
            var roleClaims = await _applicationRoleManager.GetClaimsAsync(role);
            permissions.AddRange(roleClaims.GroupBy(x => x.Value.Split('.')[3]).Select(x => new SimpleTablePermission() { Table = x.Key, Permissions = x.Select(x => x.Value).ToList() }));
        }


        return await Result<List<SimpleTablePermission>>.SuccessAsync(data: permissions);
    }
}
