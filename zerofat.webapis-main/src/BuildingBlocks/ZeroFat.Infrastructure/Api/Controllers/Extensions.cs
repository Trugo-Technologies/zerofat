namespace ZeroFat.Infrastructure.Api.Controllers;

internal static class Extensions
{
    internal static IServiceCollection AddInternalControllers(this IServiceCollection services)
    {
        services.AddControllers()
            .ConfigureApplicationPartManager(manager =>
            {
                manager.FeatureProviders.Add(new InternalControllersFeatureProvider());
            });

        return services;
    }
}
