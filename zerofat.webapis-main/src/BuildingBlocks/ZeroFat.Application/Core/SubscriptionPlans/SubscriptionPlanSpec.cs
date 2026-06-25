using Ardalis.Specification;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Domain.Core;

namespace ZeroFat.Application.Core.SubscriptionPlans;
public class SubscriptionPlansBySearchRequestSpec : EntitiesByPaginationFilterSpec<SubscriptionPlan, SubscriptionPlanSimplifyDto>
{
    public SubscriptionPlansBySearchRequestSpec(SearchSubscriptionPlansRequest request)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
             .Where(x => x.PercentageDiscount == request.PercentageDiscount, request.PercentageDiscount.HasValue)
             .Where(x => x.SubscriptionModule == request.SubscriptionModule, request.SubscriptionModule.HasValue)
             .Where(x => x.SubscriptionType == request.SubscriptionType, request.SubscriptionType.HasValue)
             .Where(x => x.IsActive == request.IsActive, request.IsActive.HasValue);
    }
}

public class SubscriptionPlanByIdSpec<T> : Specification<SubscriptionPlan, T>
{
    public SubscriptionPlanByIdSpec(DefaultIdType id) => Query.Where(p => p.Id == id);
}

