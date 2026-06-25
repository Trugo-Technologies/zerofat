using ZeroFat.Application.Common.Interfaces;
using ZeroFat.NutriPlan.Application.MenuPlanning.DailyMenus;
using ZeroFat.NutriPlan.Domain.MenuPlanning;

namespace ZeroFat.NutriPlan.Application.MenuPlanning.Menus;

public class MenuSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? NameEn { get; set; } // Name of the menu (e.g., "Weekly Low Carb Plan")
    public string? NameAr { get; set; } // Name of the menu (e.g., "Weekly Low Carb Plan")
    public bool IsPublished { get; set; } // Indicates if the menu is published or not
    public DateOnly StartDate { get; set; } // Start date of the menu
    public DateOnly? EndDate { get; set; } // End date of the menu (only for weekly menus)
}

public class MenuRawDto : MenuSimplifyDto
{
}

public class MenuAuditableDto : MenuRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class MenuDto : MenuAuditableDto
{
}

public class MenuDetailsDto : BaseEntityAuditableDetailsDto
{
    public DefaultIdType Id { get; set; }

    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public bool IsPublished { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public List<DailyMenuDto>? DailyMenus { get; set; }
}


