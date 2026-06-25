using ZeroFat.Infrastructure.FeatureFlags;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZeroFat.GymUp.Infrastructure;
using Microsoft.AspNetCore.Builder;

namespace ZeroFat.GymUp.Api;

public static class WorkoutModule
{
    public static void AddWorkoutModule(this IServiceCollection services, IConfiguration configuration)
    {
        var moduleEnabled = services.IsModuleEnabled(FeatureFlags.Module);
        if (!moduleEnabled) 
            return;

        services.AddInfrastructure(configuration);
    }

    public static WebApplication UseWorkoutModule(this WebApplication app)
    {
        return app;
    }
}
