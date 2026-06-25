using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;
using ZeroFat.NutriPlan.Domain.MealPlanning;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.MealPlanning.Extras;
public class ExtrasBySearchRequestSpec : EntitiesByPaginationFilterSpec<Extra, ExtraDto>
{
    public ExtrasBySearchRequestSpec(SearchExtrasRequest request)
        : base(request)
    {
        Query
            .Where(x => x.MealId == request.MealId, request.MealId.HasValue)
            .OrderByDescending(c => c.CreatedOn, !request.HasOrderBy());
    }
}

public class ExtraByIdSpec<T> : Specification<Extra, T>
{
    public ExtraByIdSpec(DefaultIdType id) => Query.Where(p => p.Id == id);
}

