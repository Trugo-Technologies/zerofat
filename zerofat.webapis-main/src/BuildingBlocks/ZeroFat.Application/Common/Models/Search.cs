namespace ZeroFat.Application.Common.Models;

public class Search
{
    public List<string> Fields { get; set; } = [];
    public string? Keyword { get; set; }
}

public class StrripeSessionInfo
{
    public string? SessionId { get; set; }
    public string? PaymentStatus { get; set; }
    public string? CustomerId { get; set; }
    public string? PaymentIntentId { get; set; }
    public long AmountTotal { get; set; }
    public string? Currency { get; set; }
    public List<string>? PaymentMethodTypes { get; set; }
    public string? Status { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
    public DateTime Created { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? Url { get; set; }
    public string SubscriptionId { get; set; }
}

public class StripeSubscriptionInfo
{
    public string? SubscriptionId { get; set; }
    public string? Status { get; set; }
    public DateTime? CurrentPeriodStart { get; set; }
    public DateTime? CurrentPeriodEnd { get; set; }
    public DateTime? CancelAt { get; set; }
    public bool CancelAtPeriodEnd { get; set; }
    public string? CustomerId { get; set; }
    public string? DefaultPaymentMethodId { get; set; }
    public string? LatestInvoiceId { get; set; }
    public DateTime StartDate { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
    public string? PauseCollection { get; set; }
    public string? CollectionMethod { get; set; }
    public long? DaysUntilDue { get; set; }
    public StripeInvoiceDto? LatestInvoice { get; set; }
    public List<string>? DefaultTaxRates { get; set; }
}


public class PaymentIntentResponseDTO
{
    // Basic Payment Info
    public string? Id { get; set; }                  // "pi_123456789"
    public string? Status { get; set; }               // "succeeded", "requires_action", etc.
    public long Amount { get; set; }                // Amount in cents (e.g., 2000 = $20.00)
    public string? Currency { get; set; }            // "usd", "eur", etc.
    public string? ClientSecret { get; set; }        // For client-side confirmation (e.g., "pi_..._secret_...")
    public string? LatestChargeId { get; set; }        // For client-side confirmation (e.g., "pi_..._secret_...")

    // Payment Method Details
    public string? PaymentMethodId { get; set; }     // "pm_123456789"
    public string? PaymentMethodType { get; set; }   // "card", "sepa_debit", etc.

    // Order/Customer Context
    public string? CustomerId { get; set; }          // "cus_123456789"
    public string? OrderId { get; set; }            // Your internal order ID
    public string? Description { get; set; }        // "Meal Order #12345 (2x Burger, 1x Fries)"

    // Redirect Handling (if requires_action)
    public string? RedirectUrl { get; set; }        // Only if Status = "requires_action"
    public bool RequiresConfirmation { get; set; } // True if 3D Secure/auth needed

}

public class StripeInvoiceDto
{
    public string Id { get; set; } = default!;
    public string Status { get; set; } = default!;
    public long AmountDue { get; set; }
    public long AmountPaid { get; set; }
    public long AmountRemaining { get; set; }
    public DateTime? Created { get; set; }
    public string Currency { get; set; } = default!;
    public DateTime? DueDate { get; set; }
    public string HostedInvoiceUrl { get; set; } = default!;
    public string InvoicePdf { get; set; } = default!;
    public string CustomerId { get; set; } = default!;
    public string? PaymentIntentId { get; set; }
    public string? PaymentMethodTyped { get; set; }
}
