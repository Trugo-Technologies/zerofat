using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.Contracts;
public interface IClientPortalSettingservice : ITransientService
{
    // Get Methods 
    #region get settings
    Task<int> GetOffsetSubscriptionInDays();
    Task<TimeOnly> GetCutoffTime();
    Task<NutriPlanStartegy> GetNutriPlanStartegy();
    Task<int> GetWeeklyCaloricDeficit();
    Task<int> GetWeeklyCaloricSurplus();
    Task<int> GetDefaultNutriPlanTimeAvailable();
    Task<int> GetMonthlyPackageSubsciptionDiscount();
    Task<int> GetThreeMonthlyPackageSubsciptionDiscount();
    Task<int> GetMinimumSubsciption();
    Task<int> GetOnePointEqualInMoney();
    Task<bool> GetEnableDailyMealSelections();
    #endregion

    #region set setting
    // Set Methods 
    Task SetOffsetSubscriptionInDays(int offsetSubscriptionInDays);
    Task SetCutoffTime(TimeOnly cutoffTime);
    Task SetNutriPlanStartegy(NutriPlanStartegy nutriPlanStartegy);
    Task SetWeeklyCaloricDeficit(int weeklyCaloricDeficit);
    Task SetWeeklyCaloricSurplus(int weeklyCaloricSurplus);
    Task SetDefaultNutriPlanTimeAvailable(int defaultNutriPlanTimeAvailable);

    Task SetThreeMonthlyPackageSubsciptionDiscount(int threeMonthlyPackageSubsciptionDiscount);
    Task SetMonthlyPackageSubsciptionDiscount(int monthlyPackageSubsciptionDiscount);
    Task SetMinimumSubsciption(int minimumSubsciption);
    Task SetOnePointEqualInMoney(int onePointEqualInMoney);

    Task SetEnableDailyMealSelections(bool value);
    #endregion
}
