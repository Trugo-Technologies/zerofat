using System.Reflection;
using FluentValidation;
using FSH.Framework.Infrastructure.Behaviours;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ZeroFat.GymUp.Application;

namespace ZeroFat.GymUp.Infrastructure.Mediation;

internal static class MediationModule
{
    internal static IServiceCollection AddMediationModule(this IServiceCollection services)
    {
        List<Assembly> applicationAssembly = [typeof(ApplicationModule).Assembly, typeof(MediationModule).Assembly];
        //register validators
        services.AddValidatorsFromAssemblies(applicationAssembly);

        //register mediatr
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(ApplicationModule).Assembly, typeof(MediationModule).Assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });


        return services;
    }
}
