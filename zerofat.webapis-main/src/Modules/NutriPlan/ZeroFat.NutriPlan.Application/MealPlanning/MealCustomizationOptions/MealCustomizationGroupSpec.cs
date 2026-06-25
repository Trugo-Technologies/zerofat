using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.MealCustomizationOptions;
public class MealCustomizationOptionsBySearchRequestSpec : EntitiesByPaginationFilterSpec<MealCustomizationOption, MealCustomizationOptionDto>
{
    public MealCustomizationOptionsBySearchRequestSpec(SearchMealCustomizationOptionsRequest request)
        : base(request)
    {
        Query
            .Where(x => x.MealId == request.MealId || x.MealId == null, request.MealId.HasValue)
            .OrderByDescending(c => c.CreatedOn, !request.HasOrderBy());
    }
}

public class MealCustomizationOptionByIdSpec<T> : Specification<MealCustomizationOption, T>
{
    public MealCustomizationOptionByIdSpec(DefaultIdType id)
    {
        Query.Where(p => p.Id == id);
    }
}

