namespace ZeroFat.NutriPlan.Domain.Settings;

public class MealType : ActivationEntity, IAggregateRoot
{
    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public int? Index { get; set; }
    public bool IsDefault { get; set; }
    public string? IconUrl { get; set; }
    public string? ImageUrl { get; set; }
    public TimeSpan ScheduledTime { get; set; }
    public virtual List<MealPlanMealType> MealPlanMealTypes { get; set; } = new List<MealPlanMealType>();
}
