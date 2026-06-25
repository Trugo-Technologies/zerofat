using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Enums;

namespace ZeroFat.NutriPlan.Application.Settings.Categories;

public class CategorySimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? IconUrl { get; set; }
    public string? ImageUrl { get; set; }
    public CategoryType CategoryType { get; set; }
}

public class CategoryRawDto : CategorySimplifyDto
{
}

public class CategoryAuditableDto : CategoryRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class CategoryDto : CategoryAuditableDto
{

}

public class CategoryDetailsDto : BaseEntityAuditableDetailsDto
{
    public DefaultIdType Id { get; set; }

    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? IconUrl { get; set; }
    public string? ImageUrl { get; set; }
    public CategoryType CategoryType { get; set; }

}
