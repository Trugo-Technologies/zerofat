using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Enums;

namespace ZeroFat.NutriPlan.Application.Settings.MealTypes;

public class MealTypeSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? IconUrl { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
}

public class MealTypeRawDto : MealTypeSimplifyDto
{
}

public class MealTypeAuditableDto : MealTypeRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class MealTypeDto : MealTypeAuditableDto
{

}

public class MealTypeDetailsDto : BaseEntityActivationDetailsDto
{
    public DefaultIdType Id { get; set; }

    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? IconUrl { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsDefault { get; set; }
}


