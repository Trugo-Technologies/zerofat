using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Domain.Discounts;

namespace ZeroFat.ClientPortal.Application.Discounts.DiscountRules;

public class DiscountRulesBySearchRequestSpec : EntitiesByPaginationFilterSpec<DiscountRule, DiscountRuleDto>
{
    public DiscountRulesBySearchRequestSpec(SearchDiscountRulesRequest request)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
             .Where(x => x.IsActive == request.IsActive, request.IsActive.HasValue);
    }
}

public class DiscountRuleByIdSpec<T> : Specification<DiscountRule, T>
{
    public DiscountRuleByIdSpec(DefaultIdType id)
    {
        Query.Where(p => p.Id == id);
    }
}
