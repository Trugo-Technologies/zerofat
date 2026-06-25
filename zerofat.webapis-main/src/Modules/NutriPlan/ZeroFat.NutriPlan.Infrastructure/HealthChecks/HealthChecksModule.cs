using ZeroFat.Infrastructure.HealthChecks;
using Microsoft.Extensions.DependencyInjection;

namespace ZeroFat.NutriPlan.Infrastructure.HealthChecks;
internal static class HealthChecksModule
{
    private const string DatabaseName = "NutriPlan Database";
    private const string NutriPlan = "NutriPlan";
    private const string Database = "Database";

    internal static IServiceCollection AddPersistenceHealthChecks(this IServiceCollection services, string dbProvider, string connectionString)
    {
        services.AddHealthChecks().AddHealthCheckDb(dbProvider, connectionString, DatabaseName, new[]
        {
            NutriPlan, Database
        });

        return services;
    }
}
