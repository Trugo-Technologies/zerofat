namespace ZeroFat.Infrastructure.Stripe;

public static class Extensions
{
    public static IServiceCollection AddStripe(this IServiceCollection services, IConfiguration config)
        => services.Configure<StripeSettings>(config.GetSection(nameof(StripeSettings)));
}
