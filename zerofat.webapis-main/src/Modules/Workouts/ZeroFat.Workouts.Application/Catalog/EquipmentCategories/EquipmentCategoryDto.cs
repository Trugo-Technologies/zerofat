using ZeroFat.Application.Common.Interfaces;

namespace ZeroFat.GymUp.Application.Catalog.EquipmentCategories;

public class EquipmentCategorySimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? IconUrl { get; set; }
    public bool IsActive { get; set; }
}

// public class EquipmentCategoryRawDto : EquipmentCategorySimplifyDto
// {
// }

public class EquipmentCategoryAuditableDto : EquipmentCategorySimplifyDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class EquipmentCategoryDto : EquipmentCategoryAuditableDto
{

}

public class EquipmentCategoryDetailsDto : BaseEntityActivationDetailsDto
{
    public DefaultIdType Id { get; set; }

    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? IconUrl { get; set; }
}
