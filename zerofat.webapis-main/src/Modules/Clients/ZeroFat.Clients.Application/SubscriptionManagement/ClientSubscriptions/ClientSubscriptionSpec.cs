using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.ClientSubscriptions;

public class ClientSubscriptionsBySearchRequestSpec : EntitiesByPaginationFilterSpec<ClientSubscription, ClientSubscriptionDto>
{
    public ClientSubscriptionsBySearchRequestSpec(SearchClientSubscriptionsRequest request)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
             .Where(x => x.ClientId == request.ClientId, request.ClientId.HasValue)
             .Where(x => x.PaymentStatus == request.PaymentStatus, request.PaymentStatus.HasValue)
             //.Where(x => x.SubscriptionStatus != ZeroFat.Domain.Enums.SubscriptionStatus.Pending)
             .Where(x => x.SubscriptionStatus == request.SubscriptionStatus, request.SubscriptionStatus.HasValue);
    }
}

public class ClientSubscriptionByIdSpec<T> : Specification<ClientSubscription, T>
{
    public ClientSubscriptionByIdSpec(DefaultIdType id)
    {
        Query.Where(p => p.Id == id);
    }
}

public class ClientSubscriptionByIdSpec : Specification<ClientSubscription>
{
    public ClientSubscriptionByIdSpec(DefaultIdType id)
    {
        Query.Where(p => p.Id == id).Include(x=>x.SelectedMealTypes);
    }
}

public class MealPlanByIdSpec<T> : Specification<MealPlan, T>
{
    public MealPlanByIdSpec(DefaultIdType id)
    {
        Query.Where(p => p.Id == id);
    }
}
