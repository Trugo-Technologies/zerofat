using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.MealCustomizationGroups;
public class MealCustomizationGroupsBySearchRequestSpec : EntitiesByPaginationFilterSpec<MealCustomizationGroup, MealCustomizationGroupDto>
{
    public MealCustomizationGroupsBySearchRequestSpec(SearchMealCustomizationGroupsRequest request)
        : base(request)
    {
        Query
            .OrderByDescending(c => c.CreatedOn, !request.HasOrderBy());
    }
}

public class MealCustomizationGroupByIdSpec<T> : Specification<MealCustomizationGroup, T>
{
    public MealCustomizationGroupByIdSpec(DefaultIdType id) => Query.Where(p => p.Id == id);
}

