using Ardalis.Specification;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Domain.Core;

namespace ZeroFat.Application.Core.FAQCategories;
public class FaqCategoriesBySearchRequestSpec : EntitiesByPaginationFilterSpec<FaqCategory, FaqCategoryDto>
{
    public FaqCategoriesBySearchRequestSpec(SearchFaqCategoriesRequest request)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
             .Where(x => x.IsActive == request.IsActive, request.IsActive.HasValue);
    }
}

public class FaqCategoryByIdSpec<T> : Specification<FaqCategory, T>
{
    public FaqCategoryByIdSpec(DefaultIdType id) => Query.Where(p => p.Id == id);
}

