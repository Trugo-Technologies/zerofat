using ZeroFat.Application.Common.Interfaces;

namespace ZeroFat.ClientPortal.Application.ClientManagement.ClientPaymentMethods;

public class ClientPaymentMethodSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public bool IsDefault { get; set; }
    public string? StripeId { get; set; }
    public string? Type { get; set; }
    public string? CustomerId { get; set; }
    public string? CardBrand { get; set; }
    public int? CardExpMonth { get; set; }
    public string? CardFunding { get; set; }
    public string? CardLast4 { get; set; }
    public string? CardName { get; set; }
}
public class ClientPaymentMethodRawDto : ClientPaymentMethodSimplifyDto
{
    public DefaultIdType ClientId { get; set; }
}
public class ClientPaymentMethodAuditableDto : ClientPaymentMethodRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }
    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}
public class ClientPaymentMethodDto : ClientPaymentMethodAuditableDto
{
}
public class ClientPaymentMethodDetailsDto : BaseEntityAuditableDetailsDto
{
    public DefaultIdType Id { get; set; }
    public bool IsDefault { get; set; }
    public string? StripeId { get; set; }
    public string? Type { get; set; }
    public string? CustomerId { get; set; }
    public string? CardBrand { get; set; }
    public int? CardExpMonth { get; set; }
    public string? CardFunding { get; set; }
    public string? CardLast4 { get; set; }
    public string? CardName { get; set; }
    public DefaultIdType ClientId { get; set; }
}
