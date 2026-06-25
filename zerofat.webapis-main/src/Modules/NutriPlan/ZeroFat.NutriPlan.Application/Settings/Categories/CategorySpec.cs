using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.Settings.Categories;
public class CategoriesBySearchRequestSpec : EntitiesByPaginationFilterSpec<Category, CategoryDto>
{
    public CategoriesBySearchRequestSpec(SearchCategoriesRequest request)
        : base(request)
    {
        Query.Where(x => x.CategoryType == request.CategoryType, request.CategoryType.HasValue)
            .OrderByDescending(c => c.CreatedOn, !request.HasOrderBy());
    }
}

public class CategoryByIdSpec<T> : Specification<Category, T>
{
    public CategoryByIdSpec(DefaultIdType id) => Query.Where(p => p.Id == id);
}

