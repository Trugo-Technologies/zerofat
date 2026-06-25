using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Core;

namespace ZeroFat.Application.Core.FAQs;
public class SearchFaqsRequest : PaginationFilter, IQuery<PaginationResponse<FaqSimplifyDto>>
{
    public DefaultIdType? FaqCategoryId { get; set; }
    public bool? IsActive { get; set; }
}


public class SearchFaqsRequestHandler(IReadRepository<Faq> repository) : IRequestHandler<SearchFaqsRequest, PaginationResponse<FaqSimplifyDto>>
{
    private readonly IReadRepository<Faq> _repository = repository;

    public async Task<PaginationResponse<FaqSimplifyDto>> Handle(SearchFaqsRequest request, CancellationToken cancellationToken)
         => await _repository.PaginatedListAsync(new FaqsBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);

}
