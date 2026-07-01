using ZeroFat.Api;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Infrastructure;
using ZeroFat.Users.Api;
using ZeroFat.GymUp.Api;
using ZeroFat.NutriPlan.Api;
using ZeroFat.Infrastructure.Services;
using ZeroFat.ClientPortal.Api;
using ZeroFat.Infrastructure.BackgroundProcessing.Contracts;
using ZeroFat.Infrastructure.Persistence.Configurations;
using Microsoft.Extensions.Options;
using Serilog;

namespace ZeroFat.WebAPIs;

public static class Extensions
{
    public static WebApplicationBuilder AddModules(this WebApplicationBuilder builder, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // builder.Host.UseSerilog((_, config) =>
        // {
        //     config.WriteTo.Console()
        //     .ReadFrom.Configuration(builder.Configuration);
        // });



        //define module assemblies
        builder.AddSharedInfrastructure();
        builder.Services.AddCoreModule(configuration);
        builder.Services.AddUsersModule(configuration);
        builder.Services.AddNutriPlanModule(configuration);
        builder.Services.AddClientPortalModule(configuration);
        builder.Services.AddWorkoutModule(configuration);
        builder.Services.AddServices();

        return builder;
    }

    public static WebApplication UseModules(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.UseSharedInfrastructure();
        app.UseUsersModule();
        app.UseWorkoutModule();
        app.UseCoreModule();
        app.UseNutriPlanModule();
        app.UseClientPortalModule();

        using var scope = app.Services.CreateScope();
        var scheduler = scope.ServiceProvider.GetRequiredService<IZerofatJobScheduler>();
        var seedOptions = scope.ServiceProvider.GetRequiredService<IOptions<SeedOptions>>().Value;

        var initializers = scope.ServiceProvider.GetServices<IDbInitializer>();
        foreach (var initializer in initializers)
        {
            initializer.MigrateAsync(CancellationToken.None).Wait();
        }

        if (seedOptions.EnableTestingMode)
        {
            Log.Information("Testing mode seeding enabled — running seed for all modules");
            foreach (var initializer in initializers)
            {
                initializer.SeedAsync(CancellationToken.None).Wait();
            }
        }
        else
        {
            Log.Information("Testing mode seeding disabled — skipping seed");
        }


        scheduler.ScheduleAllRecurringJobsAsync().Wait();




        return app;
    }
}
