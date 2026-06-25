using ZeroFat.Infrastructure.FeatureFlags;
using ZeroFat.Infrastructure.Core;

namespace ZeroFat.Api;

public static class CoreModule
{
    public static void AddCoreModule(this IServiceCollection services, IConfiguration configuration)
    {
        var moduleEnabled = services.IsModuleEnabled(FeatureFlags.Module);
        if (!moduleEnabled) 
            return;

        services.AddCoreInfrastructure(configuration);
    }

    public static WebApplication UseCoreModule(this WebApplication app)
    {
        return app;
    }
}
