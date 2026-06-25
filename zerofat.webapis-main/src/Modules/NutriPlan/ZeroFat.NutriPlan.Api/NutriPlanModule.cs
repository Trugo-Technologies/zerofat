using ZeroFat.Infrastructure.FeatureFlags;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZeroFat.NutriPlan.Infrastructure;
using Microsoft.AspNetCore.Builder;

namespace ZeroFat.NutriPlan.Api;

public static class NutriPlanModule
{
    public static void AddNutriPlanModule(this IServiceCollection services, IConfiguration configuration)
    {
        var moduleEnabled = services.IsModuleEnabled(FeatureFlags.Module);
        if (!moduleEnabled) 
            return;

        services.AddInfrastructure(configuration);
    }

    public static WebApplication UseNutriPlanModule(this WebApplication app)
    {
        return app;
    }
}
