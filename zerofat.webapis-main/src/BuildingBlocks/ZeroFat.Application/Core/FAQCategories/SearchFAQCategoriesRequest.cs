using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Core;

namespace ZeroFat.Application.Core.FAQCategories;
public class SearchFaqCategoriesRequest : PaginationFilter, IQuery<PaginationResponse<FaqCategoryDto>>
{
    public bool? IsActive { get; set; }
}


public class SearchFaqCategorysRequestHandler(IReadRepository<FaqCategory> repository) : IRequestHandler<SearchFaqCategoriesRequest, PaginationResponse<FaqCategoryDto>>
{
    private readonly IReadRepository<FaqCategory> _repository = repository;

    public async Task<PaginationResponse<FaqCategoryDto>> Handle(SearchFaqCategoriesRequest request, CancellationToken cancellationToken)
         => await _repository.PaginatedListAsync(new FaqCategoriesBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);

}
