using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Enums;

namespace ZeroFat.NutriPlan.Application.NutrientsAttributes;

public class NutrientsAttributeSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? Unit { get; set; }
}

public class NutrientsAttributeRawDto : NutrientsAttributeSimplifyDto
{
}

public class NutrientsAttributeAuditableDto : NutrientsAttributeRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class NutrientsAttributeDto : NutrientsAttributeAuditableDto
{

}

public class NutrientsAttributeDetailsDto : BaseEntityAuditableDetailsDto
{
    public DefaultIdType Id { get; set; }

    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? Unit { get; set; }
}
