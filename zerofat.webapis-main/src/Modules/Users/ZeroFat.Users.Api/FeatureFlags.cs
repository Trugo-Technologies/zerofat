using ZeroFat.Infrastructure.FeatureFlags;

namespace ZeroFat.Users.Api;

internal static class FeatureFlags
{
    internal static FeatureFlag Module => FeatureFlag.Define(nameof(UsersModule));
}
