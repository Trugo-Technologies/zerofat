using ZeroFat.Application.Common.Security;

namespace ZeroFat.Application.Common.Interfaces;

public interface IPermissionProvider
{
    string Module { get; }
    Task<IEnumerable<InnovatePermission>> GetPermissionsAsync();
}
