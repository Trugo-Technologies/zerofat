using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Common.Contracts;
using ZeroFat.Infrastructure.Persistence;
using ZeroFat.ClientPortal.Infrastructure.Mediation;
using ZeroFat.ClientPortal.Infrastructure.Persistence.Context;
using ZeroFat.ClientPortal.Infrastructure.Persistence.Initialization;
using ZeroFat.ClientPortal.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZeroFat.ClientPortal.Application;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Domain;
using ZeroFat.ClientPortal.Infrastructure.Services;

namespace ZeroFat.ClientPortal.Infrastructure;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<ClientPortalModuleOptions>()
            .BindConfiguration(ClientPortalModuleOptions.SectionName);

        services.AddApplicationModule();
        services.AddMediationModule();
        services.AddPersistenceModule(configuration);
        services.AddTransient<ISubscriptionPricingService, SubscriptionPricingService>();
        services.AddTransient<IDeliveryCalendarService, DeliveryCalendarService>();

        return services;
    }

    internal static IServiceCollection AddPersistenceModule(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.BindDbContext<ClientPortalContext>();
        services.AddScoped<IDbInitializer, ClientDbInitializer>();

        // Register the decorator
        foreach (var aggregateRootType in
           typeof(ModuleConstant).Assembly.GetExportedTypes()
               .Where(t => typeof(IAggregateRoot).IsAssignableFrom(t) && t.IsClass)
               .ToList())
        {
            services.AddScoped(typeof(IRepositoryWithEvents<>).MakeGenericType(aggregateRootType), serviceProvider =>
            {
                // Resolve UsersContext
                var context = serviceProvider.GetRequiredService<ClientPortalContext>();

                // Define the type for the entity, assuming you have a concrete type that implements IAggregateRoot
                // Create the UsersRepository instance
                var repositoryType = typeof(ClientPortalRepository<>).MakeGenericType(aggregateRootType);
                var repositoryInstance = ActivatorUtilities.CreateInstance(serviceProvider, repositoryType, context);

                // Create the IPRepositoryDecorator instance
                var decoratorType = typeof(IPRepositoryDecorator<,>).MakeGenericType(aggregateRootType, context.GetType());
                var decoratorInstance = ActivatorUtilities.CreateInstance(serviceProvider, decoratorType, repositoryInstance, context);

                return decoratorInstance;
            });

            services.AddScoped(typeof(IReadRepository<>).MakeGenericType(aggregateRootType), serviceProvider =>
            {
                // Resolve UsersContext
                var context = serviceProvider.GetRequiredService<ClientPortalContext>();

                // Define the type for the entity, assuming you have a concrete type that implements IAggregateRoot
                // Create the UsersRepository instance
                var repositoryType = typeof(ClientPortalRepository<>).MakeGenericType(aggregateRootType);
                var repositoryInstance = ActivatorUtilities.CreateInstance(serviceProvider, repositoryType, context);

                return repositoryInstance;
            });

            services.AddScoped(typeof(IRepository<>).MakeGenericType(aggregateRootType), serviceProvider =>
            {
                // Resolve UsersContext
                var context = serviceProvider.GetRequiredService<ClientPortalContext>();

                // Define the type for the entity, assuming you have a concrete type that implements IAggregateRoot
                // Create the UsersRepository instance
                var repositoryType = typeof(ClientPortalRepository<>).MakeGenericType(aggregateRootType);
                var repositoryInstance = ActivatorUtilities.CreateInstance(serviceProvider, repositoryType, context);

                return repositoryInstance;
            });

        }
        
        return services;
    }

   
}
