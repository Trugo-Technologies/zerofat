namespace ZeroFat.Domain.Core.Common;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class CorePermissionDefinitionAttribute : PermissionDefinitionAttribute
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1019:Define accessors for attribute arguments", Justification = "<Pending>")]
    public CorePermissionDefinitionAttribute(
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
        : base("Core", subModule, resource, withActivate, withAdd, withEdit, withDelete, withList, withDetails, withBulkActivate, withBulkDelete, otherActions)
    {
    }
}
