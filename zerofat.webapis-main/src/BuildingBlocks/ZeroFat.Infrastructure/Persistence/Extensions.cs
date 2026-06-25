using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ZeroFat.Infrastructure.Persistence.Interceptors;
using ZeroFat.Infrastructure.Persistence.Configurations;

namespace ZeroFat.Infrastructure.Persistence;
public static class Extensions
{
    private static readonly ILogger _logger = Log.ForContext(typeof(Extensions));

    public static WebApplicationBuilder ConfigureDatabase(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        var environment = builder.Environment;
        builder.Services.AddOptions<DatabaseOptions>()
            .BindConfiguration(nameof(DatabaseOptions))
            .ValidateDataAnnotations()
            .PostConfigure(config =>
            {
                _logger.Information("current db provider: {DatabaseProvider}", config.Provider);
            });
        builder.Services.AddOptions<SeedOptions>()
            .BindConfiguration(nameof(SeedOptions))
            .PostConfigure(config =>
            {
                if (!config.EnableTestingMode)
                {
                    return;
                }

                _logger.Information("testing mode seeding is enabled");

                if (environment.IsProduction())
                {
                    _logger.Warning("SeedOptions:EnableTestingMode is true in Production — test seed data will be applied on startup");
                }
            });
        builder.Services.AddTransient<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        return builder;
    }

    public static DbContextOptionsBuilder ConfigureDatabase(this DbContextOptionsBuilder builder, string dbProvider, string connectionString, string? databaseName = null)
    {
        return dbProvider.ToUpperInvariant() switch
        {
            DbProviderKeys.Npgsql => builder.UseNpgsql(connectionString, e =>
                                 e.MigrationsAssembly("Migrators.PostgreSQL")),
            DbProviderKeys.SqlServer => builder.UseSqlServer(connectionString, e =>
                                 e.MigrationsAssembly("Migrators.MSSQL")),
            DbProviderKeys.MySql => builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), e =>
                                 e.MigrationsAssembly("Migrators.MySQL")
                                  .SchemaBehavior(MySqlSchemaBehavior.Ignore)),
            DbProviderKeys.SqLite => builder.UseSqlite(connectionString, e =>
                                 e.MigrationsAssembly("Migrators.SqLite")),
            DbProviderKeys.Mongo => builder.UseMongoDB(connectionString, databaseName),
            _ => throw new InvalidOperationException($"DB Provider {dbProvider} is not supported."),
        };
    }

    public static IServiceCollection BindDbContext<TContext>(this IServiceCollection services)
    where TContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddDbContext<TContext>((sp, options) =>
        {
            var dbConfig = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            options.ConfigureDatabase(dbConfig.Provider, dbConfig.ConnectionString);
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
        });
        return services;
    }

    public static IServiceCollection BindSeperateDbContext<TContext, TZeroFatBaseModule>(this IServiceCollection services)
     where TContext : DbContext
     where TZeroFatBaseModule : ZeroFatSeperateModule
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddDbContext<TContext>((sp, options) =>
        {
            var dbConfig = sp.GetRequiredService<IOptions<TZeroFatBaseModule>>().Value;
            options.ConfigureDatabase(dbConfig.DatabaseOptions!.Provider, dbConfig.DatabaseOptions.ConnectionString, dbConfig.DatabaseOptions.DatabaseName);
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
        });
        return services;
    }
}
