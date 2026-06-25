namespace ZeroFat.Infrastructure.Paymob;

public class PaymobSettings
{
    public string? Url { get; set; }
    public string? PublicKey { get; set; }
    public string? SecretKey { get; set; }
    public string? ApiKey { get; set; }
    public string? QuicklinksLoginUrl { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public List<int>? PaymentMethods { get; set; }
    // public string? PaymentMethod { get; set; }
    public bool? IsLive { get; set; }
    public string? CreatePaymentLinkUrl { get; set; }
    public string? CreateSubscriptionPlanLinkUrl { get; set; }
    public string? UpdateSubscriptionPlanLinkUrl { get; set; }
    public string? ListSubscriptionPlanLinkUrl { get; set; }
    public string? SuspendSubscriptionPlanLinkUrl { get; set; }
    public string? ResumeSubscriptionPlanLinkUrl { get; set; }
    public string? CreateIntentionLinkUrl { get; set; }
    public string? CreateSubscriptionUrl { get; set; }
    public string? SuspendSubscriptionUrl { get; set; }
    public string? ResumeSubscriptionUrl { get; set; }
    public string? CancelSubscriptionUrl { get; set; }
    public string? UpdateSubscriptionUrl { get; set; }
    public string? GetSubscriptionUrl { get; set; }

}
