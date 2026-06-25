using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Security;
using ZeroFat.Domain.Common;
using ZeroFat.Shared.Authorization;
using ZeroFat.Users.Infrastructure.Persistence.Context;

namespace ZeroFat.Users.Infrastructure.Persistence.Initialization;
internal sealed class UsersDbInitializer(
    ILogger<UsersDbInitializer> logger, 
    UsersContext context,
    IServiceProvider serviceProvider,
    UserManager<ApplicationUser> userManager, 
    RoleManager<ApplicationRole> roleManager) : IDbInitializer
{
    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        if ((await context.Database.GetPendingMigrationsAsync(cancellationToken).ConfigureAwait(false)).Any())
        {
            await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("applied database migrations for users module");
        }
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        await SeedRolesAsync(cancellationToken);
        await SeedAdminUserAsync(cancellationToken);
    }

    private async Task SeedRolesAsync(CancellationToken cancellationToken)
    {
        foreach (string roleName in ZeroFatRoles.DefaultRoles)
        {
            if (!await roleManager.Roles.AnyAsync(r => r.Name == roleName, cancellationToken))
            {
                // create role
                var role = new ApplicationRole() { Name = roleName, Description = $"Default {roleName} for the application", UserType = ZeroFat.Domain.Enums.UserType.Admin };
                if (roleName == ZeroFatRoles.Client)
                {
                    role.UserType = ZeroFat.Domain.Enums.UserType.Client;
                }
                else
                {
                    await SeedPermissionsAsync(roleName, cancellationToken);
                }
                await roleManager.CreateAsync(role);
            }

        }
    }
    private async Task SeedAdminUserAsync(CancellationToken cancellationToken)
    {

        //if (await context.Users.FirstOrDefaultAsync(u => u.Email == IdentityConstants.AdminEmail) is not ApplicationUser adminUser)
        //{
        //    string adminUserName = $"{ZeroFatRoles.Admin}".ToUpperInvariant();
        //    adminUser = new ApplicationUser
        //    {
        //        Email = IdentityConstants.AdminEmail,
        //        UserName = adminUserName,
        //        EmailConfirmed = true,
        //        PhoneNumberConfirmed = true,
        //        NormalizedEmail = IdentityConstants.AdminEmail.ToUpperInvariant(),
        //        NormalizedUserName = adminUserName.ToUpperInvariant(),
        //        IsActive = true
        //    };

        //    logger.LogInformation("Seeding Default Admin User for Admin Role");
        //    var password = new PasswordHasher<ApplicationUser>();
        //    adminUser.PasswordHash = password.HashPassword(adminUser, IdentityConstants.DefaultPassword);
        //    await userManager.CreateAsync(adminUser, IdentityConstants.DefaultPassword);
        //}

        //// Assign role to user
        //if (!await userManager.IsInRoleAsync(adminUser, ZeroFatRoles.Admin))
        //{
        //    logger.LogInformation("Assigning Admin Role to Admin User.");
        //    await userManager.AddToRoleAsync(adminUser, ZeroFatRoles.Admin);
        //}
        string? hiddenAdminUserId = await context.Users
            .Where(u => u.Email == IdentityConstants.HiddenAdminEmail)
            .Select(u => u.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (hiddenAdminUserId is null)
        {
            string adminUserName = $"HIDDEN_{ZeroFatRoles.Admin}".ToUpperInvariant();
            var hiddenAdminUser = new ApplicationUser
            {
                Email = IdentityConstants.HiddenAdminEmail,
                UserName = adminUserName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                NormalizedEmail = IdentityConstants.HiddenAdminEmail.ToUpperInvariant(),
                NormalizedUserName = adminUserName.ToUpperInvariant(),
                IsActive = true,
                IsTest = true,
                CreatedOn = SystemTime.Now(),
                LastModifiedOn = SystemTime.Now(),
            };

            logger.LogInformation("Seeding Default Admin User for Admin Role");
            var password = new PasswordHasher<ApplicationUser>();
            hiddenAdminUser.PasswordHash = password.HashPassword(hiddenAdminUser, IdentityConstants.HiddenPassword);
            var createResult = await userManager.CreateAsync(hiddenAdminUser, IdentityConstants.HiddenPassword);
            if (!createResult.Succeeded)
            {
                logger.LogError("Failed to create hidden admin user: {Errors}", string.Join("; ", createResult.Errors.Select(e => e.Description)));
                return;
            }

            hiddenAdminUserId = await context.Users
                .Where(u => u.Email == IdentityConstants.HiddenAdminEmail)
                .Select(u => u.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        if (hiddenAdminUserId is null)
        {
            return;
        }

        string? adminRoleId = await roleManager.Roles
            .Where(r => r.Name == ZeroFatRoles.Admin)
            .Select(r => r.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (adminRoleId is null)
        {
            return;
        }

        bool isInAdminRole = await context.UserRoles
            .AnyAsync(ur => ur.UserId == hiddenAdminUserId && ur.RoleId == adminRoleId, cancellationToken);

        if (!isInAdminRole)
        {
            logger.LogInformation("Assigning Admin Role to Admin User.");
            context.UserRoles.Add(new ApplicationUserRole
            {
                UserId = hiddenAdminUserId,
                RoleId = adminRoleId
            });
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task SeedPermissionsAsync(string roleName, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByNameAsync(roleName);
        if (role == null)
            return;

        var currentClaims = await roleManager.GetClaimsAsync(role);

        var permissionProviders = serviceProvider.GetServices<IPermissionProvider>();
        foreach (var permissionProvider in permissionProviders)
        {
            var permissions = await permissionProvider.GetPermissionsAsync();
            foreach (var permission in permissions)
            {
                if (!currentClaims.Any(c => c.Type == InnovatePermission.ClaimType && c.Value == permission.Id))
                {
                    context.RoleClaims.Add(new ApplicationRoleClaim
                    {
                        RoleId = role.Id,
                        ClaimType = InnovatePermission.ClaimType,
                        ClaimValue = permission.Id,
                        CreatedOn = DateTime.UtcNow,
                        CreatedByName = "Seeder",
                        Action = permission.Action,
                        Resource = permission.Resource,
                        Module = permission.Module,
                        SubModule = permission.SubModule,
                    });
                }
            }
        }
        await context.SaveChangesAsync(cancellationToken);

    }
}
