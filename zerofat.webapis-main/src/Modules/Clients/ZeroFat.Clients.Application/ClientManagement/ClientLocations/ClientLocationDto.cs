using ZeroFat.Application.Common.Interfaces;
using ZeroFat.ClientPortal.Application.ClientManagement.Clients;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.ClientManagement;

public class ClientLocationSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
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
}

public class ClientLocationRawDto : ClientLocationSimplifyDto
{
    public Guid ClientId { get; set; }
}

public class ClientLocationAuditableDto : ClientLocationRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class ClientLocationDto : ClientLocationAuditableDto
{
    public ClientSimplifyDto? Client { get; set; }
}

public class ClientLocationDetailsDto : BaseEntityAuditableDetailsDto
{
    public DefaultIdType Id { get; set; }
    public Guid ClientId { get; set; }
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

    public ClientSimplifyDto? Client { get; set; }
}

