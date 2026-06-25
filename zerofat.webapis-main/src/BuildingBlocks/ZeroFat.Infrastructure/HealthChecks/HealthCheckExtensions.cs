using ZeroFat.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroFat.Infrastructure.HealthChecks;

public static class HealthCheckExtensions
{
    public static IHealthChecksBuilder AddHealthCheckDb(this IHealthChecksBuilder services, string dbProvider, string connectionString, string databaseName, string[] tags) =>
        dbProvider.ToUpperInvariant() switch
        {
            DbProviderKeys.Npgsql =>
                    services.AddNpgSql(connectionString, name: databaseName, tags: tags),
            DbProviderKeys.SqlServer =>
                    services.AddSqlServer(connectionString, name: databaseName, tags: tags),
            DbProviderKeys.SqLite =>
                    services,
            DbProviderKeys.MySql =>
                    services.AddMySql(connectionString, name: databaseName, tags: tags),
            DbProviderKeys.InMemory =>
                    services,
            // DbProviderKeys.Mongo =>
            //         services.AddMongoDb(connectionString, name: databaseName, tags: tags),
            _ => throw new Exception($"Storage Provider {dbProvider} is not supported.")
        };
}
