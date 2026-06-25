using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.Payments;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement;

public class PaymentsBySearchRequestSpec : EntitiesByPaginationFilterSpec<Payment, PaymentDto>
{
    public PaymentsBySearchRequestSpec(SearchPaymentsRequest request)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
             .Where(x => x.ClientId == request.ClientId, request.ClientId.HasValue)
             .Where(x => x.PaymentStatus == request.PaymentStatus, request.PaymentStatus.HasValue);
    }
}

public class PaymentByIdSpec<T> : Specification<Payment, T>
{
    public PaymentByIdSpec(Guid id) => Query.Where(p => p.Id == id);
}
