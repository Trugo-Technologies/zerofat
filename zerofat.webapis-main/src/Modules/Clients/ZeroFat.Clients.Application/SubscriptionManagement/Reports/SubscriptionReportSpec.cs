using Ardalis.Specification;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.Reports;

public class ClientSubscriptionsReportExportSpec : Specification<ClientSubscription>
{
    private const int MaxExportRows = 10_000;

    public ClientSubscriptionsReportExportSpec(SubscriptionReportFilter filter, bool isRenewReport)
    {
        Query
            .Include(x => x.Client)
            .Include(x => x.MealPlan);

        if (isRenewReport)
        {
            Query.Where(x => x.RenewalCount > 0);
        }
        else
        {
            Query.Where(x => x.RenewalCount == 0);
        }

        if (filter.ClientId.HasValue)
        {
            Query.Where(x => x.ClientId == filter.ClientId.Value);
        }

        if (filter.PaymentStatus.HasValue)
        {
            Query.Where(x => x.PaymentStatus == filter.PaymentStatus.Value);
        }

        if (filter.SubscriptionStatus.HasValue)
        {
            Query.Where(x => x.SubscriptionStatus == filter.SubscriptionStatus.Value);
        }

        if (filter.FromDate.HasValue)
        {
            var from = filter.FromDate.Value.ToDateTime(TimeOnly.MinValue);
            Query.Where(x => x.CreatedOn >= from);
        }

        if (filter.ToDate.HasValue)
        {
            var to = filter.ToDate.Value.ToDateTime(TimeOnly.MaxValue);
            Query.Where(x => x.CreatedOn <= to);
        }

        Query.OrderByDescending(x => x.CreatedOn).Take(MaxExportRows);
    }
}
