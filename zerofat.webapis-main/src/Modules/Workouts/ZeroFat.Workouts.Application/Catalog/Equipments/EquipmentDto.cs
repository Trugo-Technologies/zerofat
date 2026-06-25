using ZeroFat.Application.Common.Interfaces;
using ZeroFat.GymUp.Application.Catalog.EquipmentCategories;

namespace ZeroFat.GymUp.Application.Catalog.Equipments;

public class EquipmentSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public string? ImageUrl { get; set; }
    public string? ImageThumbnailUrl { get; set; }
    public string? ImageOptimzeUrl { get; set; }
    public bool IsActive { get; set; }
}

public class EquipmentRawDto : EquipmentSimplifyDto
{
    public DefaultIdType CategoryId { get; set; }
}

public class EquipmentAuditableDto : EquipmentRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class EquipmentDto : EquipmentAuditableDto
{

}

public class EquipmentDetailsDto : BaseEntityActivationDetailsDto
{
    public DefaultIdType Id { get; set; }

    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageThumbnailUrl { get; set; }
    public string? ImageOptimzeUrl { get; set; }
    public DefaultIdType CategoryId { get; set; }

    public EquipmentCategorySimplifyDto? EquipmentCategory { get; set; }
}
