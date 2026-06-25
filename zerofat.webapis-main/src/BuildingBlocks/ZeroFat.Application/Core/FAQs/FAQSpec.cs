using Ardalis.Specification;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Domain.Core;

namespace ZeroFat.Application.Core.FAQs;
public class FaqsBySearchRequestSpec : EntitiesByPaginationFilterSpec<Faq, FaqSimplifyDto>
{
    public FaqsBySearchRequestSpec(SearchFaqsRequest request)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
             .Where(x => x.FaqCategoryId == request.FaqCategoryId, request.FaqCategoryId.HasValue)
             .Where(x => x.IsActive == request.IsActive, request.IsActive.HasValue);
    }
}

public class FaqByIdSpec<T> : Specification<Faq, T>
{
    public FaqByIdSpec(DefaultIdType id) => Query.Where(p => p.Id == id);
}

