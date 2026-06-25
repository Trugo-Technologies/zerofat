using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Enums;

namespace ZeroFat.NutriPlan.Application.MeasurementUnits;

public class MeasurementUnitSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? Code { get; set; }
    public string? IconUrl { get; set; }
    public bool IsDefault { get; set; }
}

public class MeasurementUnitRawDto : MeasurementUnitSimplifyDto
{
}

public class MeasurementUnitAuditableDto : MeasurementUnitRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class MeasurementUnitDto : MeasurementUnitAuditableDto
{

}

public class MeasurementUnitDetailsDto : BaseEntityAuditableDetailsDto
{
    public DefaultIdType Id { get; set; }

    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
    public string? IconUrl { get; set; }
    public string? Code { get; set; }
    public bool IsDefault { get; set; }

}
