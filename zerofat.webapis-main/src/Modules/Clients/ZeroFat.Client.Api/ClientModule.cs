using ZeroFat.Infrastructure.FeatureFlags;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using ZeroFat.ClientPortal.Infrastructure;

namespace ZeroFat.ClientPortal.Api;

public static class ClientPortalModule
{
    public static void AddClientPortalModule(this IServiceCollection services, IConfiguration configuration)
    {
        var moduleEnabled = services.IsModuleEnabled(FeatureFlags.Module);
        if (!moduleEnabled) 
            return;

        services.AddInfrastructure(configuration);
    }

    public static WebApplication UseClientPortalModule(this WebApplication app)
    {
        return app;
    }
}
