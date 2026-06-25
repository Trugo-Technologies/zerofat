using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.NutriPlan.Domain.MenuPlanning;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.DailySelections;
public class DailySelectionsBySearchRequestSpec : EntitiesByPaginationFilterSpec<DailySelection, DailySelectionDto>
{
    public DailySelectionsBySearchRequestSpec(SearchDailySelectionsRequest request)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
            .Where(x => x.Date == request.Date.Value, request.Date.HasValue)
            .Where(x => x.ClientId == request.ClientId, request.ClientId.HasValue)
            .Where(x => x.ClientSubscriptionId == request.ClientSubscriptionId, request.ClientSubscriptionId.HasValue)
            .Where(x => x.ClientLocationId == request.ClientLocationId, request.ClientLocationId.HasValue)
            .Where(x => x.MealPlanId == request.MealPlanId, request.MealPlanId.HasValue);
    }
}


public class DailySelectionByDateSpec<T> : Specification<DailySelection, T>
{
    public DailySelectionByDateSpec(DateOnly date, DefaultIdType clientId)
    {
        Query.Where(p => p.Date == date && p.ClientId == clientId);
    }
}


public class DailySelectionByIdSpec<T> : Specification<DailySelection, T>
{
    public DailySelectionByIdSpec(DefaultIdType id)
    {
        Query.Where(p => p.Id == id);
    }
}

public class CurrentDailySelectionByIdSpec : Specification<DailyMenuMeal>
{
    public CurrentDailySelectionByIdSpec(DateOnly date, Guid mealTypeId, Guid mealPlanId)
    {
        Query.Include(x => x.Meal).Where(x => x.DailyMenu!.Date == date && x.DailyMenu.MealTypeId == mealTypeId && x.DailyMenu.MealPlanId == mealPlanId && x.IsDefault && x.DailyMenu.Menu!.IsPublished);
    }
}


