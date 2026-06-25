using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Common.Contracts;
using ZeroFat.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZeroFat.GymUp.Application;
using ZeroFat.GymUp.Infrastructure.Mediation;
using ZeroFat.GymUp.Infrastructure.Persistence.Context;
using ZeroFat.GymUp.Infrastructure.Persistence.Initialization;
using ZeroFat.GymUp.Infrastructure.Persistence;

namespace ZeroFat.GymUp.Infrastructure;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<WorkoutModuleOptions>()
            .BindConfiguration(WorkoutModuleOptions.SectionName);

        services.AddApplicationModule();
        services.AddMediationModule();
        services.AddPersistenceModule(configuration);

        return services;
    }

    internal static IServiceCollection AddPersistenceModule(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.BindDbContext<GymUpContext>();
        services.AddScoped<IDbInitializer, WorkoutDbInitializer>();

        // Register the decorator
        foreach (var aggregateRootType in
           typeof(Trainer).Assembly.GetExportedTypes()
               .Where(t => typeof(IAggregateRoot).IsAssignableFrom(t) && t.IsClass)
               .ToList())
        {
            services.AddScoped(typeof(IRepositoryWithEvents<>).MakeGenericType(aggregateRootType), serviceProvider =>
            {
                // Resolve UsersContext
                var context = serviceProvider.GetRequiredService<GymUpContext>();

                // Define the type for the entity, assuming you have a concrete type that implements IAggregateRoot
                // Create the UsersRepository instance
                var repositoryType = typeof(WorkoutRepository<>).MakeGenericType(aggregateRootType);
                var repositoryInstance = ActivatorUtilities.CreateInstance(serviceProvider, repositoryType, context);

                // Create the IPRepositoryDecorator instance
                var decoratorType = typeof(IPRepositoryDecorator<,>).MakeGenericType(aggregateRootType, context.GetType());
                var decoratorInstance = ActivatorUtilities.CreateInstance(serviceProvider, decoratorType, repositoryInstance, context);

                return decoratorInstance;
            });

            services.AddScoped(typeof(IRepository<>).MakeGenericType(aggregateRootType), serviceProvider =>
            {
                // Resolve UsersContext
                var context = serviceProvider.GetRequiredService<GymUpContext>();

                // Define the type for the entity, assuming you have a concrete type that implements IAggregateRoot
                // Create the UsersRepository instance
                var repositoryType = typeof(WorkoutRepository<>).MakeGenericType(aggregateRootType);
                var repositoryInstance = ActivatorUtilities.CreateInstance(serviceProvider, repositoryType, context);

                return repositoryInstance;
            });

            services.AddScoped(typeof(IReadRepository<>).MakeGenericType(aggregateRootType), serviceProvider =>
            {
                // Resolve UsersContext
                var context = serviceProvider.GetRequiredService<GymUpContext>();

                // Define the type for the entity, assuming you have a concrete type that implements IAggregateRoot
                // Create the UsersRepository instance
                var repositoryType = typeof(WorkoutRepository<>).MakeGenericType(aggregateRootType);
                var repositoryInstance = ActivatorUtilities.CreateInstance(serviceProvider, repositoryType, context);

                return repositoryInstance;
            });

        }

        // var databaseOptions = configuration.GetSection(WorkoutModuleOptions.SectionName).Get<WorkoutModuleOptions>();
        // if(databaseOptions?.DatabaseSettings != null)
        // {
        //     //services.AddPersistenceHealthChecks(databaseOptions.DatabaseSettings.Provider, databaseOptions.DatabaseSettings.ConnectionString);
        // }
        return services;
    }

   
}
