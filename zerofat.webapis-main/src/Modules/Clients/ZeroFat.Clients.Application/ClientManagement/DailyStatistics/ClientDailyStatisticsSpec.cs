using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Domain.NutritionTracking;

namespace ZeroFat.ClientPortal.Application.ClientManagement.DailyStatistics;
public class ClientDailyStatisticsBySearchRequestSpec : EntitiesByPaginationFilterSpec<ClientDailyStatistics, ClientDailyStatisticsSimplifyDto>
{
    public ClientDailyStatisticsBySearchRequestSpec(SearchClientDailyStatisticsRequest request)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
             .Where(x => x.ClientId == request.ClientId, request.ClientId.HasValue);
    }
}

public class ClientDailyStatisticsByIdSpec<T> : Specification<ClientDailyStatistics, T>
{
    public ClientDailyStatisticsByIdSpec(DefaultIdType id)
    {
        Query.Where(p => p.Id == id);
    }
}


public class CurrentClientDailyStatisticsSpec<T> : Specification<ClientDailyStatistics, T>
{
    public CurrentClientDailyStatisticsSpec(DefaultIdType clientId, DateOnly? date)
    {
        Query.Where(p => p.ClientId == clientId)
            .Where(p => p.Date == date, date.HasValue).AsNoTrackingWithIdentityResolution().OrderByDescending(x => x.Date);
    }
}

public class CurrentWeekClientDailyStatisticsSpec<T> : Specification<ClientDailyStatistics, T>
{
    public CurrentWeekClientDailyStatisticsSpec(DefaultIdType clientId)
    {
        Query.Where(p => p.ClientId == clientId && p.Date >= DateOnly.FromDateTime(DateTime.Now.AddDays(-6))).AsNoTrackingWithIdentityResolution().OrderByDescending(x => x.Date);
    }
}
