using MediatR;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.ClientPortal.Domain.Discounts;

namespace ZeroFat.ClientPortal.Application.Discounts.DiscountRules;
public class SearchDiscountRulesRequest : PaginationFilter, IQuery<PaginationResponse<DiscountRuleDto>>
{
    public bool? IsActive { get; set; }
}


public class SearchDiscountRulesRequestHandler(
    IReadRepository<DiscountRule> repository) : IRequestHandler<SearchDiscountRulesRequest, PaginationResponse<DiscountRuleDto>>
{

    public async Task<PaginationResponse<DiscountRuleDto>> Handle(SearchDiscountRulesRequest request, CancellationToken cancellationToken)
    {
        var result = await repository.PaginatedListAsync(new DiscountRulesBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);

        return result;
    }

}
