using ZeroFat.Application.Common.Interfaces;
using ZeroFat.NutriPlan.Application.MealPlanning.MealCustomizationOptions;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.MealCustomizationGroups;

public class MealCustomizationGroupSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public string? ImageUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsRequired { get; set; } // Whether user must select from this group
    public int MinSelection { get; set; } // Minimum options to select
    public int? MaxSelection { get; set; } // Maximum options allowed (0 for unlimited)

    public double CaloriesAdjustment { get; set; }
    public double FatAdjustment { get; set; }
    public double CarbsAdjustment { get; set; }
    public double ProteinAdjustment { get; set; }

}

public class MealCustomizationGroupRawDto : MealCustomizationGroupSimplifyDto
{
}

public class MealCustomizationGroupAuditableDto : MealCustomizationGroupRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class MealCustomizationGroupDto : MealCustomizationGroupAuditableDto
{
}

public class MealCustomizationGroupDetailsDto : BaseEntityAuditableDetailsDto
{
    public DefaultIdType Id { get; set; }

    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public string? ImageUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsRequired { get; set; } // Whether user must select from this group
    public int MinSelection { get; set; } // Minimum options to select
    public int? MaxSelection { get; set; } // Maximum options allowed (0 for unlimited)

    public double CaloriesAdjustment { get; set; }
    public double FatAdjustment { get; set; }
    public double CarbsAdjustment { get; set; }
    public double ProteinAdjustment { get; set; }
}

public class MealCustomizationGroupMobileDto : IDto
{
    public DefaultIdType Id { get; set; }

    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public string? ImageUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsRequired { get; set; } // Whether user must select from this group
    public int MinSelection { get; set; } // Minimum options to select
    public int? MaxSelection { get; set; } // Maximum options allowed (0 for unlimited)

    public double CaloriesAdjustment { get; set; }
    public double FatAdjustment { get; set; }
    public double CarbsAdjustment { get; set; }
    public double ProteinAdjustment { get; set; }

    public List<MealCustomizationOptionSimplifyDto> Options { get; set; } = [];

}


