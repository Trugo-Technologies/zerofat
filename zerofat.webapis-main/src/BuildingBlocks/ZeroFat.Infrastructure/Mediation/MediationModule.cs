using FluentValidation;
using FSH.Framework.Infrastructure.Behaviours;
using MediatR;
using ZeroFat.Application.Core;

namespace ZeroFat.Infrastructure.Mediation;

internal static class MediationModule
{
    internal static IServiceCollection AddMediationModule(this IServiceCollection services)
    {
        var applicationAssembly = typeof(ApplicationCoreModule).Assembly;

        //register validators
        services.AddValidatorsFromAssembly(applicationAssembly);

        //register mediatr
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(applicationAssembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        return services;
    }
}
