using ZeroFat.Infrastructure.BackgroundProcessing.Configuration;
using ZeroFat.Infrastructure.HealthChecks;

namespace ZeroFat.Infrastructure.BackgroundProcessing.HealthChecks;

internal static class BackgroundProcessingHealthChecksModule
{
    private const string Shared = "Shared";
    private const string BackgroundProcessing = "Background Processing";
    private const string Database = "Database";

    internal static IHealthChecksBuilder AddBackgroundProcessingHealthCheck(this IHealthChecksBuilder services, IConfiguration configuration)
    {
        var backgroundProcessingConfiguration = new BackgroundProcessingConfiguration(configuration);

        services
            .AddHealthCheckDb(backgroundProcessingConfiguration.StorageProvider, backgroundProcessingConfiguration.ConnectionString, "Hangfire db",
            [
                Shared, Database
            ])
            .AddHangfire(hangfire =>
            {
                hangfire.MaximumJobsFailed = 5;
                hangfire.MinimumAvailableServers = 1;
            }, BackgroundProcessing, tags:
            [
                 Shared,
            ]);

        return services;
    }

    
}
