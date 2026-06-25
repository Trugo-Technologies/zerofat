using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;
using ZeroFat.NutriPlan.Domain.MenuPlanning;

namespace ZeroFat.NutriPlan.Application.MenuPlanning.DailyMenuMeals;
public class DailyMenuMealsBySearchRequestSpec : EntitiesByPaginationFilterSpec<DailyMenuMeal, DailyMenuMealDto>
{
    public DailyMenuMealsBySearchRequestSpec(SearchDailyMenuMealsRequest request)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
            .Where(x => x.DailyMenu.Date == request.Date!.Value, request.Date.HasValue)
            .Where(x => x.DailyMenu.MealTypeId == request.MealTypeId, request.MealTypeId.HasValue)
            .Where(x => x.DailyMenu.MenuId == request.MenuId, request.MenuId.HasValue)
            .Where(x => x.DailyMenu.MealPlanId == request.MealPlanId, request.MealPlanId.HasValue);
    }
}

public class CurrentDailyMenuMealsBySearchRequestSpec : EntitiesByPaginationFilterSpec<DailyMenuMeal, MenuMealDto>
{
    public CurrentDailyMenuMealsBySearchRequestSpec(SearchCurrentDailyMenuMealsRequest request)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
            .Where(x => x.DailyMenu.Date >= DateOnly.FromDateTime(DateTime.Now.Date) && x.DailyMenu.Date <= DateOnly.FromDateTime(DateTime.Now.Date).AddDays(8))
            .Where(x => x.DailyMenu.MealTypeId == request.MealTypeId, request.MealTypeId.HasValue)
            .Where(x => x.DailyMenu.MealPlanId == request.MealPlanId, request.MealPlanId.HasValue);
    }
}

public class DailyMenuMealByIdSpec<T> : Specification<DailyMenuMeal, T>
{
    public DailyMenuMealByIdSpec(Guid id) => Query.Where(p => p.Id == id);
}



public class CopyDailyMenuMealSpec : Specification<DailyMenuMeal, DailyMenuMealSimplifyDto>
{
    public CopyDailyMenuMealSpec(Guid menuId, Guid mealPlanId, Guid mealTypeId, DateOnly date)
    {
        Query.Where(p => p.DailyMenu.MenuId == menuId && p.DailyMenu.MealPlanId == mealPlanId && p.DailyMenu.MealTypeId == mealTypeId && p.DailyMenu.Date == date);
    }
}


