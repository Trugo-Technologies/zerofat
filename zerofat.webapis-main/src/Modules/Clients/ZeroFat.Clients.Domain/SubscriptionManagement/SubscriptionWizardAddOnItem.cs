namespace ZeroFat.ClientPortal.Domain.SubscriptionManagement;

public class SubscriptionWizardAddOnItem
{
    public DefaultIdType ItemId { get; set; }
    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public SubscriptionAddOnCategory Category { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
