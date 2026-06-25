namespace ZeroFat.Infrastructure.HealthChecks.FeatureFlag;

using Infrastructure.FeatureFlags;

internal static class FeatureFlags
{
    internal static FeatureFlag HealthChecksIU => FeatureFlag.Define(nameof(HealthChecksIU));
}
