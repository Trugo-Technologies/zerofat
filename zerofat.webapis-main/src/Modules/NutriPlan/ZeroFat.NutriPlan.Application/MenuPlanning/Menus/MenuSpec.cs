using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;
using ZeroFat.NutriPlan.Domain.MenuPlanning;

namespace ZeroFat.NutriPlan.Application.MenuPlanning.Menus;
public class MenusBySearchRequestSpec : EntitiesByPaginationFilterSpec<Menu, MenuDto>
{
    public MenusBySearchRequestSpec(SearchMenusRequest request)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy());
    }
}

public class MenuByIdSpec<T> : Specification<Menu, T>
{
    public MenuByIdSpec(Guid id) => Query.Where(p => p.Id == id);
}

