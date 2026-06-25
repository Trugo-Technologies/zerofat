using ZeroFat.Infrastructure.FeatureFlags;
using ZeroFat.NutriPlan.Application.Contracts;

namespace ZeroFat.NutriPlan.Api;

internal static class FeatureFlags
{
    internal static FeatureFlag Module => FeatureFlag.Define(nameof(NutriPlanModule));
}
