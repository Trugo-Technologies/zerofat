using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.ClientOrders;

public class ClientOrderDto : IDto
{
    public DefaultIdType ClientSubscriptionId { get; set; } // Foreign key to the client

    public DateOnly StartDate { get; set; } // Start date of the subscription
    public DateOnly EndDate { get; set; } // End date of the subscription

    public decimal TotalCost { get; set; } // Total cost of the subscription
    public string? PaymentQuickLink { get; set; }

    public PaymentStatus PaymentStatus { get; set; } // Payment status of the subscription (e.g., "Paid", "Pending")
    public bool IsRecurring { get; set; } // Whether the subscription automatically renews

    public DefaultIdType ClientId { get; set; } // Foreign key to the client
    public string? PaymentOrderId { get; set; }
}
