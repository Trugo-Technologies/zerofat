using Microsoft.EntityFrameworkCore;
using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Domain.SubscriptionManagement;

[Index(nameof(Date))]
public class DailySelection : AuditableEntity
{
    public DailySelection()
    {
        DailyMealSelections = [];
    }

    public DateOnly Date { get; set; } // Date of the meal selection

    public PreferredMealTime DeliveryTime { get; set; } // The client's preferred delivery time for this meal

    public bool HasCutlery { get; set; }
    public double TotalCalories { get; set; } // Total calories in the selected meal
    public double TotalFats { get; set; } // Total fat content in the selected meal
    public double TotalCarbohydrates { get; set; } // Total carbohydrate content in the selected meal
    public double TotalProteins { get; set; } // Total protein content in the selected meal

    public DailySelectionStatus DailySelectionStatus { get; set; }
    public DateOnly? ReplacementDate { get; set; } // The new date replacing this paused day

    // Navigation properties
    public virtual List<DailyMealSelection> DailyMealSelections { get; set; } // The daily menu for the date

    public DefaultIdType MealPlanId { get; set; } // Foreign key to the client
    public DefaultIdType ClientSubscriptionId { get; set; } // Foreign key to the client

    public DefaultIdType ClientLocationId { get; set; } // Foreign key to the client
    public virtual ClientLocation? ClientLocation { get; set; } // Foreign key to the client

    public DefaultIdType ClientId { get; set; } // Foreign key to the client
    public virtual Client? Client { get; set; } // The client who made the selection
}
