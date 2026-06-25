using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.Meals;
public class MealsBySearchRequestSpec : EntitiesByPaginationFilterSpec<Meal, MealDto>
{
    public MealsBySearchRequestSpec(SearchMealsRequest request)
        : base(request)
    {
        Query
            .Where(x => x.IsAddOn == request.IsAddOn!.Value, request.IsAddOn.HasValue)
            .Where(x => x.OrginalMealId == null)
            .OrderByDescending(c => c.CreatedOn, !request.HasOrderBy());
    }
}

public class MealByIdSpec<T> : Specification<Meal, T>
{
    public MealByIdSpec(Guid id)
    {
        Query.Where(p => p.Id == id);
    }
}

