using System.ComponentModel.DataAnnotations.Schema;
using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Domain.SubscriptionManagement;

public class ClientOrder : AuditableEntity
{
    public ClientOrder()
    {
        ClientOrderItems = [];
    }
    public DefaultIdType ClientSubscriptionId { get; set; } // Foreign key to the client
    public virtual ClientSubscription? ClientSubscription { get; set; } // Foreign key to the client

    public DateOnly StartDate { get; set; } // Start date of the subscription
    public DateOnly EndDate { get; set; } // End date of the subscription

    public decimal TotalCost { get; set; } // Total cost of the subscription
    public string? PaymentQuickLink { get; set; }
    public string? MealDescription { get; set; }

    public PaymentStatus PaymentStatus { get; set; } // Payment status of the subscription (e.g., "Paid", "Pending")
    public bool IsRecurring { get; set; } // Whether the subscription automatically renews

    public List<ClientOrderItem> ClientOrderItems { get; set; }

    public DefaultIdType ClientId { get; set; } // Foreign key to the client
    [ForeignKey("ClientId")]
    public virtual Client? Client { get; set; } // Foreign key to the client

}


public class ClientOrderItem
{
    public int QuantityPerDay { get; set; } // The number of meals of this type the client has selected (e.g., 3 breakfasts)
    public double Price { get; set; } // The number of meals of this type the client has selected (e.g., 3 breakfasts)
    public DefaultIdType MealId { get; set; } // Foreign key to the client
}
