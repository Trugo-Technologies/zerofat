using ZeroFat.Infrastructure.FeatureFlags;

namespace ZeroFat.Infrastructure.Observability;
internal static class FeatureFlags
{
    internal static FeatureFlag ObservabilityModule => FeatureFlag.Define(nameof(ObservabilityModule));
}
