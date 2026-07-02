using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Common.Contracts;
using ZeroFat.Infrastructure.Persistence;
using ZeroFat.Users.Application;
using ZeroFat.Users.Infrastructure.HealthChecks;
using ZeroFat.Users.Infrastructure.Identity;
using ZeroFat.Users.Infrastructure.Mediation;
using ZeroFat.Users.Infrastructure.Persistence.Context;
using ZeroFat.Users.Infrastructure.Persistence.Initialization;
using ZeroFat.Users.Infrastructure.Persistence;
using ZeroFat.Users.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZeroFat.Users.Infrastructure.Auth.Jwt;

namespace ZeroFat.Users.Infrastructure;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<UsersModuleOptions>()
            .BindConfiguration(UsersModuleOptions.SectionName);

        services.AddApplicationModule();
        services.AddMediationModule();
        services.AddJwtTokenAuthorizationModule(configuration);
        services.AddPersistenceModule(configuration);
        services.AddIdentity();
        services.AddTransient<IPushNotificationService, Notifications.FcmPushNotificationService>();

        return services;
    }

    internal static IServiceCollection AddPersistenceModule(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.BindDbContext<UsersContext>();
        services.AddScoped<IDbInitializer, UsersDbInitializer>();
        services.AddScoped(typeof(IRepository<>), typeof(UsersRepository<>));
        services.AddScoped(typeof(IReadRepository<>), typeof(UsersRepository<>));

        // Register the decorator
        foreach (var aggregateRootType in
           typeof(ApplicationUser).Assembly.GetExportedTypes()
               .Where(t => typeof(IAggregateRoot).IsAssignableFrom(t) && t.IsClass)
               .ToList())
        {
            services.AddScoped(typeof(IRepositoryWithEvents<>).MakeGenericType(aggregateRootType), serviceProvider =>
            {
                // Resolve UsersContext
                var context = serviceProvider.GetRequiredService<UsersContext>();

                // Define the type for the entity, assuming you have a concrete type that implements IAggregateRoot
                // Create the UsersRepository instance
                var repositoryType = typeof(UsersRepository<>).MakeGenericType(aggregateRootType);
                var repositoryInstance = ActivatorUtilities.CreateInstance(serviceProvider, repositoryType, context);

                // Create the IPRepositoryDecorator instance
                var decoratorType = typeof(IPRepositoryDecorator<,>).MakeGenericType(aggregateRootType, context.GetType());
                var decoratorInstance = ActivatorUtilities.CreateInstance(serviceProvider, decoratorType, repositoryInstance, context);

                return decoratorInstance;
            });

            services.AddScoped(typeof(IRepository<>).MakeGenericType(aggregateRootType), serviceProvider =>
            {
                // Resolve UsersContext
                var context = serviceProvider.GetRequiredService<UsersContext>();

                // Define the type for the entity, assuming you have a concrete type that implements IAggregateRoot
                // Create the UsersRepository instance
                var repositoryType = typeof(UsersRepository<>).MakeGenericType(aggregateRootType);
                var repositoryInstance = ActivatorUtilities.CreateInstance(serviceProvider, repositoryType, context);

                return repositoryInstance;
            });

            services.AddScoped(typeof(IReadRepository<>).MakeGenericType(aggregateRootType), serviceProvider =>
            {
                // Resolve UsersContext
                var context = serviceProvider.GetRequiredService<UsersContext>();

                // Define the type for the entity, assuming you have a concrete type that implements IAggregateRoot
                // Create the UsersRepository instance
                var repositoryType = typeof(UsersRepository<>).MakeGenericType(aggregateRootType);
                var repositoryInstance = ActivatorUtilities.CreateInstance(serviceProvider, repositoryType, context);

                return repositoryInstance;
            });

        }

        // var databaseOptions = configuration.GetSection(UsersModuleOptions.SectionName).Get<UsersModuleOptions>();
        // if(databaseOptions?.DatabaseSettings != null)
        // {
        //     services.AddPersistenceHealthChecks(databaseOptions.DatabaseSettings.Provider, databaseOptions.DatabaseSettings.ConnectionString);
        // }
        return services;
    }

   
}
