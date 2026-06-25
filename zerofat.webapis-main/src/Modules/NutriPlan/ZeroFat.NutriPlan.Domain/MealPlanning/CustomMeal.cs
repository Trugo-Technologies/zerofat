namespace ZeroFat.NutriPlan.Domain.MealPlanning;

public class CustomMeal : AuditableEntity
{
    public CustomMeal()
    {
        SelectedOptions = [];
    }

    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public string? SpecialInstructions { get; set; }
    public string? ImageUrl { get; set; }

    // Reference to the original meal this was customized from
    public DefaultIdType? BaseMealId { get; set; }
    public virtual Meal? BaseMeal { get; set; }

    // The customer who created this custom meal
    public DefaultIdType ClientId { get; set; }

    // Selected customization options
    public virtual ICollection<CustomMealOption> SelectedOptions { get; set; }

    // Calculated properties
    public double TotalCalories { get; set; }
    public double TotalFat { get; set; }
    public double TotalCarbs { get; set; }
    public double TotalProtein { get; set; }
    public decimal TotalPrice { get; set; }
    // Similar calculated properties for other nutritional values
}

public class CustomMealOption : Entity
{
    public DefaultIdType CustomMealId { get; set; }
    public virtual CustomMeal? CustomMeal { get; set; }

    public DefaultIdType OptionId { get; set; }
    public virtual MealCustomizationOption? Option { get; set; }

    // You can add quantity if options can be added multiple times
    public int Quantity { get; set; } = 1;
}
