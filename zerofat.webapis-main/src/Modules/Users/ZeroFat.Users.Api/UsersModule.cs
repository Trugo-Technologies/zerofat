using ZeroFat.Infrastructure.FeatureFlags;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZeroFat.Users.Infrastructure;
using Microsoft.AspNetCore.Builder;

namespace ZeroFat.Users.Api;

public static class UsersModule
{
    public static void AddUsersModule(this IServiceCollection services, IConfiguration configuration)
    {
        var moduleEnabled = services.IsModuleEnabled(FeatureFlags.Module);
        if (!moduleEnabled) 
            return;

        // services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddInfrastructure(configuration);
    }

    public static WebApplication UseUsersModule(this WebApplication app)
    {
        return app;
    }
}
