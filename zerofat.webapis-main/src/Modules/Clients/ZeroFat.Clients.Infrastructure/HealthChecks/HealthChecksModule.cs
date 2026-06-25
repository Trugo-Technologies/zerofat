using ZeroFat.Infrastructure.HealthChecks;
using Microsoft.Extensions.DependencyInjection;

namespace ZeroFat.ClientPortal.Infrastructure.HealthChecks;
internal static class HealthChecksModule
{
    private const string DatabaseName = "Clients Database";
    private const string Clients = "Clients";
    private const string Database = "Database";

    internal static IServiceCollection AddPersistenceHealthChecks(this IServiceCollection services, string dbProvider, string connectionString)
    {
        services.AddHealthChecks().AddHealthCheckDb(dbProvider, connectionString, DatabaseName, new[]
        {
            Clients, Database
        });

        return services;
    }
}
