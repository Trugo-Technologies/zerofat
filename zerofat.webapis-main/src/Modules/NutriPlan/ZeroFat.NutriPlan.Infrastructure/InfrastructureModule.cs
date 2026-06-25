using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Common.Contracts;
using ZeroFat.Infrastructure.Persistence;
using ZeroFat.NutriPlan.Infrastructure.Mediation;
using ZeroFat.NutriPlan.Infrastructure.Persistence.Context;
using ZeroFat.NutriPlan.Infrastructure.Persistence.Initialization;
using ZeroFat.NutriPlan.Infrastructure.Persistence;
using ZeroFat.NutriPlan.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZeroFat.NutriPlan.Application;
using ZeroFat.NutriPlan.Domain;

namespace ZeroFat.NutriPlan.Infrastructure;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<NutriPlanModuleOptions>()
            .BindConfiguration(NutriPlanModuleOptions.SectionName);

        services.AddApplicationModule();
        services.AddMediationModule();
        services.AddPersistenceModule(configuration);

        return services;
    }

    internal static IServiceCollection AddPersistenceModule(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.BindDbContext<NutriPlanContext>();
        services.AddScoped<IDbInitializer, NutriPlanDbInitializer>();

        // services.AddScoped(typeof(IRepository<>), typeof(NutriPlanRepository<>));
        // services.AddScoped(typeof(IReadRepository<>), typeof(NutriPlanRepository<>));

        // Register the decorator
        foreach (var aggregateRootType in
           typeof(ModuleConstant).Assembly.GetExportedTypes()
               .Where(t => typeof(IAggregateRoot).IsAssignableFrom(t) && t.IsClass)
               .ToList())
        {
            services.AddScoped(typeof(IRepositoryWithEvents<>).MakeGenericType(aggregateRootType), serviceProvider =>
            {
                // Resolve UsersContext
                var context = serviceProvider.GetRequiredService<NutriPlanContext>();

                // Define the type for the entity, assuming you have a concrete type that implements IAggregateRoot
                // Create the UsersRepository instance
                var repositoryType = typeof(NutriPlanRepository<>).MakeGenericType(aggregateRootType);
                var repositoryInstance = ActivatorUtilities.CreateInstance(serviceProvider, repositoryType, context);

                // Create the IPRepositoryDecorator instance
                var decoratorType = typeof(IPRepositoryDecorator<,>).MakeGenericType(aggregateRootType, context.GetType());
                var decoratorInstance = ActivatorUtilities.CreateInstance(serviceProvider, decoratorType, repositoryInstance, context);

                return decoratorInstance;
            });

            services.AddScoped(typeof(IReadRepository<>).MakeGenericType(aggregateRootType), serviceProvider =>
            {
                // Resolve UsersContext
                var context = serviceProvider.GetRequiredService<NutriPlanContext>();

                // Define the type for the entity, assuming you have a concrete type that implements IAggregateRoot
                var repositoryType = typeof(NutriPlanRepository<>).MakeGenericType(aggregateRootType);
                var repositoryInstance = ActivatorUtilities.CreateInstance(serviceProvider, repositoryType, context);
                
                return repositoryInstance;
            });

            services.AddScoped(typeof(IRepository<>).MakeGenericType(aggregateRootType), serviceProvider =>
            {
                // Resolve UsersContext
                var context = serviceProvider.GetRequiredService<NutriPlanContext>();

                // Define the type for the entity, assuming you have a concrete type that implements IAggregateRoot
                var repositoryType = typeof(NutriPlanRepository<>).MakeGenericType(aggregateRootType);
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
