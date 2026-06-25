using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Domain.ClientManagement;

public class ClientLocation : AuditableEntity, IAggregateRoot
{
    public DefaultIdType ClientId { get; set; }
    public AddressType? Type { get; set; }

    public double? Longitude { get; set; }
    public double? Latitude { get; set; }
    public string? FullAddressEn { get; set; }
    public string? FullAddressAr { get; set; }
    public string? Area { get; set; }
    public string? Building { get; set; }
    public string? Office { get; set; }
    public string? Street { get; set; }
    public string? PostalCode { get; set; }
    public string? NotesFromClient { get; set; }
    public string? NotesFromZeroFat { get; set; }
    public virtual Client Client { get; set; } = default!;
}


