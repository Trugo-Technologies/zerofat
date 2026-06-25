using ZeroFat.Infrastructure.Api.Cors.Configurations;
using Microsoft.AspNetCore.Builder;
using System.Configuration;

namespace ZeroFat.Infrastructure.Api.Cors;
internal static class CorsExtensions
{
    private const string CorsPolicy = nameof(CorsPolicy);

    internal static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration config)
    {
        var corsConfiguration = new CorsConfiguration(config);
        var origins = new List<string>();

        if (corsConfiguration.Development is not null)
            origins.AddRange(corsConfiguration.Development.Split(';', StringSplitOptions.RemoveEmptyEntries));
        if (corsConfiguration.Production is not null)
            origins.AddRange(corsConfiguration.Production.Split(';', StringSplitOptions.RemoveEmptyEntries));

        return services.AddCors(opt =>
            opt.AddPolicy(CorsPolicy, policy =>
                policy.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithOrigins(origins.ToArray())));
    }

    internal static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app) =>
        app.UseCors(CorsPolicy);
}
