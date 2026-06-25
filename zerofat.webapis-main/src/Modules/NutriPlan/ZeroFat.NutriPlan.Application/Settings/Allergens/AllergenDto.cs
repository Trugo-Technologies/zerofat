using ZeroFat.Application.Common.Interfaces;

namespace ZeroFat.NutriPlan.Application.Settings.Allergens;

public class AllergenSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? IconUrl { get; set; }
    public bool? IsAllergic { get; set; }
}

public class AllergenRawDto : AllergenSimplifyDto
{
}

public class AllergenAuditableDto : AllergenRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class AllergenDto : AllergenAuditableDto
{

}

public class AllergenDetailsDto : BaseEntityAuditableDetailsDto
{
    public DefaultIdType Id { get; set; }

    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? IconUrl { get; set; }

}

