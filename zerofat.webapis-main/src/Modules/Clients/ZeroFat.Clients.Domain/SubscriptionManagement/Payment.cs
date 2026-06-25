using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Domain.SubscriptionManagement;

/// <summary>
/// Represents a payment made by the client for a subscription.
/// </summary>
public class Payment : AuditableEntity
{
    public DateTime? PaymentDate { get; set; } // Date the payment was made
    public decimal Amount { get; set; } // The amount paid
    public string? PaymentMethod { get; set; } // The method of payment (e.g., Credit Card, PayPal)

    public PaymentStatus PaymentStatus { get; set; } // Status of the payment (e.g., Completed, Failed, Pending)

    public string? TransactionId { get; set; } // The transaction ID from the payment gateway
    public string? OrderId { get; set; }

    public string? InvoiceNumber { get; set; } // Invoice number for this payment

    public string? PaymentGateway { get; set; } // Payment gateway used for the transaction (e.g., Stripe, PayPal)
    public string? Currency { get; set; } // The currency in which the payment was made (e.g., USD, EUR)

    public bool IsDiscountApplied { get; set; } // Indicates if a discount was applied to this payment
    public string? Reason { get; set; }
    public string? Notes { get; set; } // Additional notes or details about the payment

    public DefaultIdType ClientId { get; set; } // Foreign key to the client
    public virtual Client? Client { get; set; } // Foreign key to the client

    public DefaultIdType ClientSubscriptionId { get; set; } // Foreign key to the client
    public virtual ClientSubscription? ClientSubscription { get; set; } // Foreign key to the client
}
