using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.Settings.MealPlans;
public class MealPlansBySearchRequestSpec : EntitiesByPaginationFilterSpec<MealPlan, MealPlanDto>
{
    public MealPlansBySearchRequestSpec(SearchMealPlansRequest request)
        : base(request)
    {
        Query
            .Where(x=>x.IsActive == request.IsActive, request.IsActive.HasValue)
            .OrderByDescending(c => c.CreatedOn, !request.HasOrderBy());
    }
}

public class MealPlansByFilterSpec : Specification<MealPlan, MealPlanDto>
{
    public MealPlansByFilterSpec(bool? isActive)
    {
        Query
            .Include(x => x.MealPlanMealTypes)
            .ThenInclude(x => x.MealType)
            .Where(x => x.IsActive == isActive!.Value, isActive.HasValue)
            .OrderByDescending(x => x.CreatedOn);
    }
}

public class MealPlanByIdSpec<T> : Specification<MealPlan, T>
{
    public MealPlanByIdSpec(DefaultIdType id) => Query.Where(p => p.Id == id);
}


public class MealPlansSpec : Specification<MealPlan>
{
    public MealPlansSpec() => Query.Where(p => p.IsActive).Include(x=>x.MealPlanMealTypes).ThenInclude(x=>x.MealType);
}
