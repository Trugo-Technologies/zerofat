using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Localization;
using ZeroFat.Infrastructure.Paymob;

namespace ZeroFat.Infrastructure.Paymob;

public static class Extensions
{
    public static IServiceCollection AddPaymob(this IServiceCollection services, IConfiguration config)
        => services.Configure<PaymobSettings>(config.GetSection(nameof(PaymobSettings)));
}
