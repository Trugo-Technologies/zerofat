using ZeroFat.Infrastructure.FeatureFlags;

namespace ZeroFat.ClientPortal.Api;

internal static class FeatureFlags
{
    internal static FeatureFlag Module => FeatureFlag.Define(nameof(ClientPortalModule));
}
