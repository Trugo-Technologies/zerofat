using Hangfire;
using Hangfire.Console;
using Hangfire.Console.Extensions;
using Hangfire.MySql;
using Hangfire.PostgreSql;
using Hangfire.SQLite;
using Hangfire.SqlServer;
using HangfireBasicAuthenticationFilter;
using ZeroFat.Infrastructure.BackgroundProcessin.Filters;
using ZeroFat.Infrastructure.BackgroundProcessing.Configuration;
using ZeroFat.Infrastructure.BackgroundProcessing.Contracts;
using ZeroFat.Infrastructure.BackgroundProcessing.Storage;
using ZeroFat.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using UPC.WebApis.Infrastructure.BackgroundJobs;

namespace ZeroFat.Infrastructure.BackgroundProcessing;
internal static class Extensions
{
    internal static IServiceCollection AddBackgroundProcessing(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfireServer(options => configuration.GetSection("HangfireSettings:Server").Bind(options));
        services.AddHangfireConsoleExtensions();
        services.AddHttpContextAccessor();

        var backgroundProcessingConfiguration = new BackgroundProcessingConfiguration(configuration);

        services.AddSingleton<JobActivator, IPJobActivator>();
        services.AddScoped<IBackgroundJobScheduler, BackgroundJobScheduler>();
        services.AddScoped<IBackgroundProcessQueue, BackgroundProcessQueue>();

        services.AddScoped<IZerofatJobScheduler, ZerofatJobScheduler>();
        // Register scheduler

        // services.AddScoped<IRecurringBackgroundJobScheduler, RecurringBackgroundJobScheduler>();

        services.AddHangfire((provider, hangfireConfig) => hangfireConfig
            .UseDatabase(backgroundProcessingConfiguration.StorageProvider, backgroundProcessingConfiguration.ConnectionString, configuration)
            .UseFilter(new IPJobFilter(provider))
            .UseFilter(new LogJobFilter())
            .UseConsole());

        return services;
    }

    private static IGlobalConfiguration UseDatabase(this IGlobalConfiguration hangfireConfig, string dbProvider, string connectionString, IConfiguration config) =>
       dbProvider.ToUpperInvariant() switch
       {
           DbProviderKeys.Npgsql =>
                hangfireConfig.UsePostgreSqlStorage(configure => configure.UseNpgsqlConnection(connectionString), config.GetSection("HangfireSettings:Storage:Options").Get<PostgreSqlStorageOptions>()),
           DbProviderKeys.SqlServer =>
                hangfireConfig.UseSqlServerStorage(connectionString, config.GetSection("HangfireSettings:Storage:Options").Get<SqlServerStorageOptions>()),
           DbProviderKeys.SqLite =>
                hangfireConfig.UseSQLiteStorage(connectionString, config.GetSection("HangfireSettings:Storage:Options").Get<SQLiteStorageOptions>()),
           DbProviderKeys.MySql =>
                hangfireConfig.UseStorage(new MySqlStorage(connectionString, config.GetSection("HangfireSettings:Storage:Options").Get<MySqlStorageOptions>())),
           DbProviderKeys.InMemory =>
                hangfireConfig.UseInMemoryStorage(),
           DbProviderKeys.Mongo =>
                hangfireConfig.UseMongoAsStorage(connectionString),

           _ => throw new Exception($"Hangfire Storage Provider {dbProvider} is not supported.")
       };

    internal static void UseBackgroundProcessing(this IApplicationBuilder app, IConfiguration config)
    {
        var dashboardOptions = config.GetSection("HangfireSettings:Dashboard").Get<DashboardOptions>();
        if (dashboardOptions != null)
        {
            dashboardOptions.Authorization =
            [
               new HangfireCustomBasicAuthenticationFilter
               {
                    User = config.GetSection("HangfireSettings:Credentials:User").Value,
                    Pass = config.GetSection("HangfireSettings:Credentials:Password").Value
               }
            ];
        }
        app.UseHangfireDashboard(config["HangfireSettings:Route"], dashboardOptions);
    }

    internal static IEndpointRouteBuilder MapBackgroundProcessing(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHangfireDashboard();

        return endpoints;
    }
}
