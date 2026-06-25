using Microsoft.Extensions.Logging;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Infrastructure.SMS.Etisalat;
using ZeroFat.Infrastructure.SMS.Twilio;

namespace ZeroFat.Infrastructure.SMS;

internal static class Extensions
{
    public static IServiceCollection AddTwilioService(this IServiceCollection services, TwilioOptions options)
    {
        services.AddSingleton<ISMSService>(new TwilioService(options));

        return services;
    }

    public static IServiceCollection AddEtisalatService(this IServiceCollection services, EtisalatOptions options)
    {
        services.AddSingleton<ISMSService>(sp =>
            new EtisalatService(
                options,
                sp.GetRequiredService<ILogger<EtisalatService>>(),
                sp.GetRequiredService<HttpClient>()
            ));

        return services;
    }

    public static IServiceCollection AddSMSService(this IServiceCollection services, SMSOptions options)
    {
        if (options.UsedTwillo())
        {
            services.AddTwilioService(options.Twillo);
        }
        else if (options.UsedAmazon())
        {
            services.AddEtisalatService(options.Etisalat);
        }
        else
        {
            // services.AddLocalStorageManager(options.Local);
        }


        return services;
    }
}
