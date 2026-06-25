
namespace ZeroFat.Shared.Paymob;

public class QuickLinkLoginResponse
{
    public dynamic? Profile { get; set; }
    public string? Token { get; set; }
    public string? Profile_token { get; set; }
}

public class CreateQuickLinkResponse
{
    public int Id { get; set; }
    public string? Currency { get; set; }
    public object? Client_info { get; set; }
    public string? Reference_id { get; set; }
    public double? Amount_cents { get; set; }
    public string? Payment_link_image { get; set; }
    public string? Descriptione { get; set; }
    public string? Created_at { get; set; }
    public string? Expires_at { get; set; }
    public string? Client_url { get; set; }
    public int? Origin { get; set; }
    public string? Merchant_staff_tag { get; set; }
    public string? State { get; set; }
    public string? Paid_at { get; set; }
    public string? Redirection_url { get; set; }
    public string? Notification_url { get; set; }
    public int? Order { get; set; }
}


public class CreateIntentionLinkResponse
{
    public List<PaymentKey>? PaymentKeys { get; set; }
    public string? Id { get; set; }
    public IntentionDetail? Intention_detail { get; set; }
    public string? Client_secret { get; set; }
    public List<PaymentMethod>? PaymentMethods { get; set; }
    public string? SpecialReference { get; set; }
    public bool Confirmed { get; set; }
    public string? Status { get; set; }
    public DateTime Created { get; set; }
    public string? CardDetail { get; set; }
    public string? Object { get; set; }
}

public class PaymentKey
{
    public int Integration { get; set; }
    public string? Key { get; set; }
    public string? GatewayType { get; set; }
    public string? IframeId { get; set; }
}

public class IntentionDetail
{
    public int Amount { get; set; }
    public List<PaymobItem> Items { get; set; }
    public string? Currency { get; set; }
}

public class PaymobItem
{
    public string? name { get; set; }
    public int amount { get; set; }
    public string? description { get; set; }
    public int quantity { get; set; }
}

public class PaymentMethod
{
    public int IntegrationId { get; set; }
    public string? Alias { get; set; }
    public string? Name { get; set; }
    public string? MethodType { get; set; }
    public string? Currency { get; set; }
    public bool Live { get; set; }
}

public class Plan
{
    public int Id { get; set; }
    public int Frequency { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? Name { get; set; }
    public int? ReminderDays { get; set; } // Nullable int
    public int? RetrialDays { get; set; }  // Nullable int
    public string? PlanType { get; set; }
    public int? NumberOfDeductions { get; set; } // Nullable int
    public int? AmountCents { get; set; } // Nullable int
    public bool UseTransactionAmount { get; set; }
    public bool is_active { get; set; }
    public string? WebhookUrl { get; set; }
    public int Integration { get; set; }
    public int? Fee { get; set; } // Nullable int
}

public class Subscription
{
  public int Id {  get; set; }
  public ClientInfo client_info {  get; set; }
  public int Frequency {  get; set; }
  public string? Name {  get; set; }
  public string? Created_at {  get; set; }
  public string? Updated_at {  get; set; }
  public string? Reminder_days {  get; set; }
  public string? Retrial_days {  get; set; }
  public string? Starts_at {  get; set; }
  public int Plan_id {  get; set; }
  public string State {  get; set; }
  public int Amount_cents {  get; set; }
  public string? Next_billing {  get; set; }
  public string? Reminder_date {  get; set; }
  public string? Ends_at {  get; set; }
  public string? Resumed_at {  get; set; }
  public string? Suspended_at {  get; set; }
  public string? Webhook_url {  get; set; }
  public int Integration {  get; set; }
  public int Initial_transaction {  get; set; }
}
public class ClientInfo
{
    public string Email {  get; set; }
    public string Full_name {  get; set; }
    public string phone_number {  get; set; }
}

