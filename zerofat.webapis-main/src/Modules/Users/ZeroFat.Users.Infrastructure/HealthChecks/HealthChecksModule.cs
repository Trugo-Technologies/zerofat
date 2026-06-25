using ZeroFat.Infrastructure.HealthChecks;
using Microsoft.Extensions.DependencyInjection;

namespace ZeroFat.Users.Infrastructure.HealthChecks;
internal static class HealthChecksModule
{
    private const string DatabaseName = "Users Database";
    private const string Users = "Users";
    private const string Database = "Database";

    internal static IServiceCollection AddPersistenceHealthChecks(this IServiceCollection services, string dbProvider, string connectionString)
    {
        services.AddHealthChecks().AddHealthCheckDb(dbProvider, connectionString, DatabaseName, new[]
        {
            Users, Database
        });

        return services;
    }
}
