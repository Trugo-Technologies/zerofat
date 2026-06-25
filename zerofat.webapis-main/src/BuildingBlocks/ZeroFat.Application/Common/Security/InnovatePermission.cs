using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroFat.Domain.Common;

namespace ZeroFat.Application.Common.Security;
public class InnovatePermission
{
    public const string ClaimType = "Permission";

    public InnovatePermission(string description, string module, string subModule, string resource, string action)
    {
        Module = module;
        SubModule = subModule;
        Resource = resource;
        Action = action;
        Description = description;
        Id = NameFor(module, subModule, resource, action);
    }

    public string Id { get; private set; }
    public string Module { get; private set; }
    public string SubModule { get; private set; }
    public string Resource { get; private set; }
    public string Action { get; private set; }
    public string Description { get; private set; }

    public static string NameFor(string module, string subModule, string resource, string action) => $"{ClaimType}.{module}.{subModule}.{resource}.{action}";

}



public static class InnovatePermissions
{
    public static IEnumerable<InnovatePermission> GroupResourcePermissions(string module, string subModule, string resource,
        bool withActivate = true,
        bool withAdd = true,
        bool withEdit = true,
        bool withDelete = true,
        bool withList = true,
        bool withDetails = true,
        bool withBulkActivate = false,
        bool withBulkDelete = false)
    {
        List<InnovatePermission> permissions = new List<InnovatePermission>();

        if (withAdd)
            permissions.Add(new($"{InnovateAction.Add} {resource} in {subModule} Module", module, subModule, resource, InnovateAction.Add));

        if (withEdit)
            permissions.Add(new($"{InnovateAction.Edit} {resource} in {subModule} Module", module, subModule, resource, InnovateAction.Edit));

        if (withList)
            permissions.Add(new($"{InnovateAction.List} {resource} in {subModule} Module", module, subModule, resource, InnovateAction.List));

        if (withDetails)
            permissions.Add(new($"{InnovateAction.Details} {resource} in {subModule} Module", module, subModule, resource, InnovateAction.Details));

        if (withDelete)
            permissions.Add(new($"{InnovateAction.Delete} {resource} in {subModule} Module", module, subModule, resource, InnovateAction.Delete));

        if (withBulkDelete)
            permissions.Add(new($"{InnovateAction.BulkDelete} {resource} in {subModule} Module", module, subModule, resource, InnovateAction.BulkDelete));

        if (withActivate)
            permissions.Add(new($"{InnovateAction.Activate} {resource} in {subModule} Module", module, subModule, resource, InnovateAction.Activate));

        if (withBulkActivate)
            permissions.Add(new($"{InnovateAction.BulkActivate} {resource} in {subModule} Module", module, subModule, resource, InnovateAction.BulkActivate));

        return permissions.ToArray();
    }

    public static IEnumerable<InnovatePermission> GroupSubModulePermissions(string module, string subModule, string[] resources)
    {
        List<InnovatePermission> permissions = new List<InnovatePermission>();
        foreach (var resource in resources)
            permissions.AddRange(GroupResourcePermissions(module, subModule, resource));
        return permissions;
    }

}

public class InnovateModulePermission
{
    public string? Module { get; set; }
    public List<InnovateTablePermission>? Tables { get; set; }
}

public class InnovateTablePermission
{
    public string? Table { get; set; }
    public List<InnovatePermission>? InnovatePermissions { get; set; }
}


public class SimpleTablePermission
{
    public string? Table { get; set; }
    public List<string>? Permissions { get; set; }
}
