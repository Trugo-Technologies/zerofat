namespace ZeroFat.NutriPlan.Domain.MenuPlanning;

public class Menu : AuditableEntity, IAggregateRoot
{
    public Menu()
    {
        DailyMenus = new List<DailyMenu>();
    }
    public string? NameEn { get; set; } // Name of the menu (e.g., "Weekly Low Carb Plan")
    public string? NameAr { get; set; } // Name of the menu (e.g., "Weekly Low Carb Plan")
    public bool IsPublished { get; set; } // Indicates if the menu is published or not
    public DateOnly StartDate { get; set; } // Start date of the menu
    public DateOnly? EndDate { get; set; } // End date of the menu (only for weekly menus)

    // If it's a weekly menu, it will contain a list of daily menus
    public virtual ICollection<DailyMenu> DailyMenus { get; set; } // List of daily menus for each day of the week
}
