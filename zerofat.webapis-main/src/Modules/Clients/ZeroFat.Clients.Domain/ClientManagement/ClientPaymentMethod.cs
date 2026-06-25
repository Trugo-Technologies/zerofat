namespace ZeroFat.ClientPortal.Domain.ClientManagement;

public class ClientPaymentMethod : AuditableEntity, IAggregateRoot
{
    public bool IsDefault { get; set; }
    public string? StripeId { get; set; }
    public DefaultIdType? ClientId { get; set; }
    public virtual Client? Client { get; set; }
    public string? Type { get; set; }
    public string? CustomerId { get; set; }
    public string? CardBrand { get; set; }
    public int? CardExpMonth { get; set; }
    public string? CardFunding { get; set; }
    public string? CardLast4 { get; set; }
    public string? CardName { get; set; }
}
