using ZeroFat.Application.Common.Interfaces;

namespace ZeroFat.GymUp.Application.Catalog.BodyParts;

public class BodyPartSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public bool IsActive { get; set; }
}

public class BodyPartRawDto : BodyPartSimplifyDto
{
}

public class BodyPartAuditableDto : BodyPartRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class BodyPartDto : BodyPartAuditableDto
{

}

public class BodyPartDetailsDto : BaseEntityActivationDetailsDto
{
    public DefaultIdType Id { get; set; }

    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
}
