using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace ZeroFat.Infrastructure.Persistence;

public static class ModelBuilderExtensions
{
    public static ModelBuilder AppendGlobalQueryFilter<TInterface>(this ModelBuilder modelBuilder, Expression<Func<TInterface, bool>> filter)
    {
        // get a list of entities without a baseType that implement the interface TInterface
        var entities = modelBuilder.Model.GetEntityTypes()
            .Where(e => e.BaseType is null && e.ClrType.GetInterface(typeof(TInterface).Name) is not null)
            .Select(e => e.ClrType);

        foreach (var entity in entities)
        {
            var parameterType = Expression.Parameter(modelBuilder.Entity(entity).Metadata.ClrType);
            var filterBody = ReplacingExpressionVisitor.Replace(filter.Parameters.Single(), parameterType, filter.Body);

            // get the existing query filter
            if (modelBuilder.Entity(entity).Metadata.GetQueryFilter() is { } existingFilter)
            {
                var existingFilterBody = ReplacingExpressionVisitor.Replace(existingFilter.Parameters.Single(), parameterType, existingFilter.Body);

                // combine the existing query filter with the new query filter
                filterBody = Expression.AndAlso(existingFilterBody, filterBody);
            }

            // apply the new query filter
            modelBuilder.Entity(entity).HasQueryFilter(Expression.Lambda(filterBody, parameterType));
        }

        return modelBuilder;
    }
    private static MethodInfo GetApplyConfigurationMethod()
    {
        return typeof(ModelBuilder)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy)
            .FirstOrDefault(m => m.IsGenericMethod && m.Name.Equals("ApplyConfiguration", StringComparison.OrdinalIgnoreCase) &&
                                    m.GetParameters().FirstOrDefault()?.ParameterType.Name == "IEntityTypeConfiguration`1")
            ?? throw new InvalidOperationException("ApplyConfiguration method not found on ModelBuilder.");
    }

    private static IEnumerable<Type> GetConfigurationTypes(Assembly assembly, string? configNamespace)
    {
        return assembly.GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract && !type.ContainsGenericParameters &&
                            (configNamespace == null || type.Namespace == configNamespace));
    }

    public static void ApplyEntityConfigurations(this ModelBuilder modelBuilder, Assembly assembly, string? configNamespace = null)
    {
        var applyConfigMethod = GetApplyConfigurationMethod();

        foreach (var type in GetConfigurationTypes(assembly, configNamespace))
        {
            var entityTypeConfig = type.GetInterfaces()
                .FirstOrDefault(iface => iface.IsConstructedGenericType &&
                                            iface.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>));

            if (entityTypeConfig != null)
            {
                var entityType = entityTypeConfig.GenericTypeArguments[0];
                var applyConcreteMethod = applyConfigMethod.MakeGenericMethod(entityType);
                applyConcreteMethod.Invoke(modelBuilder, [Activator.CreateInstance(type)!]);
                Console.WriteLine($"Applied configuration: {type.Name}");
            }
        }
    }

    public static void ExcludeEntitiesFromMigrations(this ModelBuilder modelBuilder, Assembly assembly, string? configNamespace = null)
    {
        var applyConfigMethod = GetApplyConfigurationMethod();

        foreach (var type in GetConfigurationTypes(assembly, configNamespace))
        {
            var entityTypeConfig = type.GetInterfaces()
                .FirstOrDefault(iface => iface.IsConstructedGenericType &&
                                            iface.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>));

            if (entityTypeConfig != null)
            {
                var entityType = entityTypeConfig.GenericTypeArguments[0];
                var applyConcreteMethod = applyConfigMethod.MakeGenericMethod(entityType);
                applyConcreteMethod.Invoke(modelBuilder, [Activator.CreateInstance(type)!]);
                modelBuilder.Entity(entityType).ToTable(string.Empty); // Exclude from migrations
                Console.WriteLine($"Applied configuration: {type.Name}");
            }
        }
    }
}
