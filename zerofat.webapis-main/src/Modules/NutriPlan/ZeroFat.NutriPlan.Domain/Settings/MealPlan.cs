using ZeroFat.Domain.Enums;

namespace ZeroFat.NutriPlan.Domain.Settings;
public class MealPlan : ActivationEntity, IAggregateRoot
{
    public MealPlan()
    {
        MealPlanMealTypes = [];
        Images = [];
    }
    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public string? DescriptionEn { get; set; }
    public string? DescriptionAr { get; set; }
    public string? Code { get; set; }
    public string? ImageUrl { get; set; }
    public List<string> Images { get; set; }

    public DietitianGoal DefaultDietitianGoal { get; set; }

    public decimal? CarbPercentage { get; set; }
    public decimal? ProteinPercentage { get; set; }
    public decimal? FatPercentage { get; set; }

    public string? StripeId { get; set; }
    public virtual List<MealPlanMealType> MealPlanMealTypes { get; set; }
}


