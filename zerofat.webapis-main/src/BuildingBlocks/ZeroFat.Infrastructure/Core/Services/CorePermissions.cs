using System.Reflection;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Security;
using ZeroFat.Domain.Core.Common;

namespace ZeroFat.Infrastructure.Core.Services;

public class CorePermissions : IPermissionProvider
{
    private const string _module = "Core";
    public string Module => _module;

    public Task<IEnumerable<InnovatePermission>> GetPermissionsAsync()
    {
        var permissions = new List<InnovatePermission>();

        // Get all types in the current assembly
        var types = typeof(ICoreDomain).Assembly.GetExportedTypes()
              .Where(t => typeof(ICoreAggregateRoot).IsAssignableFrom(t) && t.IsClass)
              .ToList();

        foreach (var type in types)
        {
            // Fetch all CorePermissionDefinitionAttribute attributes from the type
            var permissionAttributes = type.GetCustomAttributes<CorePermissionDefinitionAttribute>();

            foreach (var permissionAttribute in permissionAttributes)
            {
                var resource = permissionAttribute.Resource;
                var module = permissionAttribute.Module;
                var subModule = permissionAttribute.SubModule;
                var actions = permissionAttribute.Actions;

                // Generate permissions based on actions
                foreach (var action in actions)
                {
                    permissions.Add(new InnovatePermission(
                        $"{action} {resource} in {subModule} Module",
                        module,
                        subModule,
                        resource,
                        action
                    ));
                }
            }
        }

        return Task.FromResult<IEnumerable<InnovatePermission>>(permissions);
    }
}
