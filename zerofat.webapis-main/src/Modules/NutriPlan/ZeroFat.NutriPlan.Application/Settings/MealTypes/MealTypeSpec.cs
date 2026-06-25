using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.Settings.MealTypes;
public class MealTypesBySearchRequestSpec : EntitiesByPaginationFilterSpec<MealType, MealTypeDto>
{
    public MealTypesBySearchRequestSpec(SearchMealTypesRequest request)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy());
    }
}

public class MealTypeByIdSpec<T> : Specification<MealType, T>
{
    public MealTypeByIdSpec(DefaultIdType id) => Query.Where(p => p.Id == id);
}

