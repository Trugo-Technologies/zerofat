using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Common.Contracts;

namespace ZeroFat.Application.Common.Models;

public class BaseEntityAuditableDetailsDto : IAuditableEntity, IDto
{
    public DefaultIdType CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public DefaultIdType LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public string? LastModifiedByName { get; set; }

    public DateTime? DeletedOn { get; set; }
    public DefaultIdType? DeletedBy { get; set; }
    public string? DeletedByName { get; set; }
}
