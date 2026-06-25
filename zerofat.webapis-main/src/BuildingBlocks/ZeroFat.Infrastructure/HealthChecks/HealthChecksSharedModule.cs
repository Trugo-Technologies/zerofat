using ZeroFat.Infrastructure.HealthChecks.Memory;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using HealthChecks.UI.Client;
using ZeroFat.Infrastructure.BackgroundProcessing.HealthChecks;
using ZeroFat.Infrastructure.HealthChecks.Configuration;

namespace ZeroFat.Infrastructure.HealthChecks;
internal static class HealthChecksSharedModule
{
    internal static IServiceCollection AddSharedHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
                .AddMemoryHealthCheck()
                .AddBackgroundProcessingHealthCheck(configuration);

        // Settings settings = configuration.GetSection("HealthChecksSettings").Bind();
        // var settings = configuration.GetSection("HealthChecksSettings").Get<Settings>();

        services.AddHealthChecksUI()
                .AddInMemoryStorage();

        return services;
    }

    internal static void UseSharedHealthChecks(this IApplicationBuilder app, IConfiguration config)
    {
        var healthCheckConfiguration = new HealthCheckConfiguration(config);
        var options = new HealthCheckOptions
        {
            AllowCachingResponses = true,
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        };
        app.UseHealthChecks(healthCheckConfiguration.ApiPath, options);
        app.UseHealthChecksUI(options =>
        {
            options.UIPath = healthCheckConfiguration.UIPath;
        });
    }

    public static object MapSharedHealthChecks(
        this IEndpointRouteBuilder endpoints, IConfiguration config)
    {
        var healthCheckConfiguration = new HealthCheckConfiguration(config);
        endpoints.MapHealthChecks(healthCheckConfiguration.RootPath);

        return endpoints;
    }
}
