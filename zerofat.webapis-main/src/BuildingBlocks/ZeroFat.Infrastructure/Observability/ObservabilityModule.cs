using ZeroFat.Infrastructure.FeatureFlags;

namespace ZeroFat.Infrastructure.Observability;

internal static class ObservabilityModule
{
    internal static IServiceCollection AddObservability(this IServiceCollection services)
    {
        var moduleEnabled = services.IsModuleEnabled(FeatureFlags.ObservabilityModule);
        if (!moduleEnabled) 
            return services;

        // services.AzureApplicationInsights();

        return services;
    }
}
