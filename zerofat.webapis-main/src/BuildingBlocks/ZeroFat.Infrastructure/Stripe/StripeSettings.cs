namespace ZeroFat.Infrastructure.Stripe;

public class StripeSettings
{
    public string? SecretKey { get; set; }
    public string? SuccessUrlDomain { get; set; }
    public string? Currency { get; set; }
    public string? CheckoutCompletedSecretKey { get; set; }
}
