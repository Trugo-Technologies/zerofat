using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;
using ZeroFat.NutriPlan.Domain.MenuPlanning;

namespace ZeroFat.NutriPlan.Application.MenuPlanning.DailyMenus;
public class DailyMenusBySearchRequestSpec : EntitiesByPaginationFilterSpec<DailyMenu, DailyMenuDto>
{
    public DailyMenusBySearchRequestSpec(SearchDailyMenusRequest request)
        : base(request)
    {
        Query
            .Where(x => x.MenuId == request.MenuId, request.MenuId.HasValue)
            .Where(x => x.MealPlanId == request.MealPlanId, request.MealPlanId.HasValue)
            .Where(x => x.MealTypeId == request.MealTypeId, request.MealTypeId.HasValue)
            .Where(x => x.Date == request.Date, request.Date.HasValue)
            .OrderByDescending(c => c.CreatedOn, !request.HasOrderBy());
    }
}

public class DailyMenuByIdSpec<T> : Specification<DailyMenu, T>
{
    public DailyMenuByIdSpec(Guid id) => Query.Where(p => p.Id == id);
}

