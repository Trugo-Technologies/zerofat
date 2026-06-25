using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using ZeroFat.Infrastructure.Audits.Persistence;
using ZeroFat.Infrastructure.Persistence;

namespace ZeroFat.Infrastructure.Audits;

public static class InfrastructureAuditModule
{
    public static IServiceCollection AddAuditModule(this IServiceCollection services,
      IConfiguration configuration)
    {
        // services.AddOptions<FmsModuleOptions>()
        //     .BindConfiguration(FmsModuleOptions.SectionName);
        // services.AddAuditMediationModule();
        // services.AddTransient<IAuditService, AuditService>();

        services.Configure<AuditModuleOptions>(configuration.GetSection(AuditModuleOptions.SectionName));

        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        var dbOptions = configuration.GetSection(AuditModuleOptions.SectionName).Get<AuditModuleOptions>();
        services.BindSeperateDbContext<AuditContext, AuditModuleOptions>();

        return services;
    }
}
