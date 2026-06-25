using ZeroFat.Infrastructure.FeatureFlags;

namespace ZeroFat.GymUp.Api;

internal static class FeatureFlags
{
    internal static FeatureFlag Module => FeatureFlag.Define(nameof(WorkoutModule));
}
