using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Domain.Settings;

public class Allergen : AuditableEntity, IAggregateRoot
{
    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public string? IconUrl { get; set; }
    public virtual List<IngredientAllergen> IngredientAllergens { get; set; }
}
