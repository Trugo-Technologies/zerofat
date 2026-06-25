using ZeroFat.Infrastructure.HealthChecks;
using Microsoft.Extensions.DependencyInjection;

namespace ZeroFat.GymUp.Infrastructure.HealthChecks;
internal static class HealthChecksModule
{
    private const string DatabaseName = "Workouts Database";
    private const string Workouts = "Workouts";
    private const string Database = "Database";

    internal static IServiceCollection AddPersistenceHealthChecks(this IServiceCollection services, string dbProvider, string connectionString)
    {
        services.AddHealthChecks().AddHealthCheckDb(dbProvider, connectionString, DatabaseName, new[]
        {
            Workouts, Database
        });

        return services;
    }
}
