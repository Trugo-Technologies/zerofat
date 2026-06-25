using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Models;

namespace ZeroFat.Application.Core.FAQCategories;

public class FaqCategorySimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public bool? IsActive { get; set; }
}

public class FaqCategoryRawDto : FaqCategorySimplifyDto
{
}

public class FaqCategoryAuditableDto : FaqCategoryRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class FaqCategoryDto : FaqCategoryAuditableDto
{

}

public class FaqCategoryDetailsDto : BaseEntityActivationDetailsDto
{
    public DefaultIdType Id { get; set; }

    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
}
