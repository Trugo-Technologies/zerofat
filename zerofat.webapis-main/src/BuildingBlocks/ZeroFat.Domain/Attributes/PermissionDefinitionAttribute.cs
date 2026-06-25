using ZeroFat.Domain.Common;

namespace ZeroFat.Domain.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class PermissionDefinitionAttribute : Attribute
{
    public string Module { get; }
    public string Resource { get; }
    public string SubModule { get; }
    public string[] Actions { get; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1019:Define accessors for attribute arguments", Justification = "<Pending>")]
    public PermissionDefinitionAttribute(
        string module,
        string subModule,
        string resource,
        bool withActivate = true,
        bool withAdd = true,
        bool withEdit = true,
        bool withDelete = true,
        bool withList = true,
        bool withDetails = true,
        bool withBulkActivate = true,
        bool withBulkDelete = true,
        params string[] otherActions)
    {
        Module = module;
        SubModule = subModule;
        Resource = resource;
        Actions = GetActions(withActivate, withAdd, withEdit, withDelete, withList, withDetails, withBulkActivate, withBulkDelete, otherActions);
    }


    private static string[] GetActions(bool withActivate, bool withAdd, bool withEdit, bool withDelete, bool withList, bool withDetails, bool withBulkActivate, bool withBulkDelete, params string[] otherActions)
    {
        var actions = new List<string>();
        if (withAdd) actions.Add(InnovateAction.Add);
        if (withEdit) actions.Add(InnovateAction.Edit);
        if (withList) actions.Add(InnovateAction.List);
        if (withDetails) actions.Add(InnovateAction.Details);
        if (withDelete) actions.Add(InnovateAction.Delete);
        if (withBulkDelete) actions.Add(InnovateAction.BulkDelete);
        if (withActivate) actions.Add(InnovateAction.Activate);
        if (withBulkActivate) actions.Add(InnovateAction.BulkActivate);

        // Add custom actions
        if (otherActions != null && otherActions.Length > 0)
        {
            actions.AddRange(otherActions);
        }

        return actions.ToArray();
    }
}
