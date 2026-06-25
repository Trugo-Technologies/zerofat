using Ardalis.Specification;
using ZeroFat.Domain.Core;

namespace ZeroFat.ClientPortal.Application.Settings.ClientPortalSettings;
public class ClientPortalSettingByNameSpec : Specification<Setting>
{
    public ClientPortalSettingByNameSpec(string proertyName) => Query.Where(p => p.ApplicationModule == ZeroFat.Domain.Enums.ApplicationModule.ClientPortal && p.PropertyName.ToLower() == proertyName.ToLower());
}

