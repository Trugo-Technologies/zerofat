namespace ZeroFat.Domain.Common.Contracts;

public interface IAuditableEntity
{
    public DefaultIdType CreatedBy { get; set; }
    public DateTime CreatedOn { get; }
    public string? CreatedByName { get; set; }

    public DefaultIdType LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public string? LastModifiedByName { get; set; }

}