using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.Settings.ClientPortalSettings;

public class ClientPortalSetting
{
    public int OffsetSubscriptionInDays { get; set; }
    public NutriPlanStartegy NutriPlanStartegy { get; set; }
    public int WeeklyCaloricDeficit { get; set; }
    public int WeeklyCaloricSurplus { get; set; }
    public int DefaultNutriPlanTimeAvailable { get; set; }
    public int MonthlyPackageSubsciptionDiscount { get; set; }
    public int MinimumSubsciption { get; set; }
    public int ThreeMonthlyPackageSubsciptionDiscount { get; set; }
    public int OnePointEqualInMoney { get; set; }
    public bool EnableDailyMealSelections { get; set; }
}
