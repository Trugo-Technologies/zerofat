using ZeroFat.Application.Common.Persistence;
using ZeroFat.Infrastructure.Persistence;
using ZeroFat.Infrastructure.Mediation;
using ZeroFat.Application.Core;
using ZeroFat.Infrastructure.Core.Persistence.Initialization;
using ZeroFat.Infrastructure.Core.Services;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Infrastructure.Core.Persistence.Context;
using ZeroFat.Infrastructure.Core.Persistence;
using ZeroFat.Domain.Core.Common;

namespace ZeroFat.Infrastructure.Core;

public static class InfrastructureCoreModule
{
    public static IServiceCollection AddCoreInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<CoreModuleOptions>()
            .BindConfiguration(CoreModuleOptions.SectionName);

        services.AddApplicationCoreModule();
        services.AddMediationModule();
        services.AddPersistenceModule(configuration);

        return services;
    }

    internal static IServiceCollection AddPersistenceModule(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.BindDbContext<CoreContext>();
        services.AddScoped<IDbInitializer, CoreDbInitializer>();
        services.AddScoped<IPermissionProvider, CorePermissions>();
        services.AddKeyedScoped<IPermissionProvider, CorePermissions>("Core");

        foreach (var aggregateRootType in
          typeof(ICoreDomain).Assembly.GetExportedTypes()
              .Where(t => typeof(ICoreAggregateRoot).IsAssignableFrom(t) && t.IsClass)
              .ToList())
        {
            var repoType = typeof(CoreRepository<>).MakeGenericType(aggregateRootType);
            var decoratorType = typeof(IPRepositoryDecorator<,>).MakeGenericType(aggregateRootType, typeof(CoreContext));

            services.AddScoped(typeof(IRepositoryWithEvents<>).MakeGenericType(aggregateRootType), decoratorType);
            services.AddScoped(typeof(IReadRepository<>).MakeGenericType(aggregateRootType), repoType);
            services.AddScoped(typeof(IRepository<>).MakeGenericType(aggregateRootType), repoType);
        }

        return services;
    }


}
