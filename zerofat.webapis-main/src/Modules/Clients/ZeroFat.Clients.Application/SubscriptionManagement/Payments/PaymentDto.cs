using ZeroFat.Application.Common.Interfaces;
using ZeroFat.ClientPortal.Application.ClientManagement.Clients;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.ClientSubscriptions;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement;

public class PaymentSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }

    public DateTime? PaymentDate { get; set; } // Date the payment was made
    public decimal Amount { get; set; } // The amount paid
    public string? PaymentMethod { get; set; } // The method of payment (e.g., Credit Card, PayPal)

    public PaymentStatus PaymentStatus { get; set; } // Status of the payment (e.g., Completed, Failed, Pending)

    public string? TransactionId { get; set; } // The transaction ID from the payment gateway
    public string? InvoiceNumber { get; set; } // Invoice number for this payment

    public string? PaymentGateway { get; set; } // Payment gateway used for the transaction (e.g., Stripe, PayPal)
    public string? Currency { get; set; } // The currency in which the payment was made (e.g., USD, EUR)
    public string? Reason { get; set; }

    public string? Notes { get; set; } // Additional notes or details about the payment

    public DefaultIdType ClientId { get; set; } // Foreign key to the client
}

public class PaymentRawDto : PaymentSimplifyDto
{
}

public class PaymentAuditableDto : PaymentRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class PaymentDto : PaymentAuditableDto
{
    public ClientSimplifyDto? Client { get; set; }
    public ClientSubscriptionDto? ClientSubscription { get; set; }
}

public class PaymentDetailsDto : BaseEntityActivationDetailsDto
{
    public DefaultIdType Id { get; set; }

    public DateTime? PaymentDate { get; set; } // Date the payment was made
    public decimal Amount { get; set; } // The amount paid
    public string? PaymentMethod { get; set; } // The method of payment (e.g., Credit Card, PayPal)

    public PaymentStatus PaymentStatus { get; set; } // Status of the payment (e.g., Completed, Failed, Pending)

    public string? TransactionId { get; set; } // The transaction ID from the payment gateway
    public string? InvoiceNumber { get; set; } // Invoice number for this payment

  

    public string? PaymentGateway { get; set; } // Payment gateway used for the transaction (e.g., Stripe, PayPal)
    public string? Currency { get; set; } // The currency in which the payment was made (e.g., USD, EUR)
    public string? Reason { get; set; }



    public string? Notes { get; set; } // Additional notes or details about the payment

    public DefaultIdType ClientId { get; set; } // Foreign key to the client
    public string? PaymentQuickLink { get; set; }
    public ClientSimplifyDto? Client { get; set; }
    public ClientSubscriptionDto? ClientSubscription { get; set; }
}
