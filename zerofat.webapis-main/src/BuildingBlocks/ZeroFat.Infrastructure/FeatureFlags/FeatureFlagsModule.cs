namespace ZeroFat.Infrastructure.FeatureFlags;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

internal static class FeatureFlagsModule
{
    internal static void AddFeatureFlags(this IServiceCollection services)
    {
        services.AddFeatureManagement();
        services.AddSingleton<IFeatureFlagsChecker, FeatureFlagsChecker>();
    }
}
