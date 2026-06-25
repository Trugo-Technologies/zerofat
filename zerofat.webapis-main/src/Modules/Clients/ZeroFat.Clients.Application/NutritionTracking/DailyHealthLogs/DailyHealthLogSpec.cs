using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Domain.NutritionTracking;

namespace ZeroFat.ClientPortal.Application.NutritionTracking.DailyHealthLogs;
public class DailyHealthLogBySearchRequestSpec : EntitiesByPaginationFilterSpec<DailyHealthLog, DailyHealthLogSimplifyDto>
{

    public DailyHealthLogBySearchRequestSpec(SearchDailyHealthLogsRequest request, DefaultIdType? clientId)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
             .Where(x => x.ClientId == clientId, clientId.HasValue);
    }
    
}

public class DailyHealthLogByIdSpec<T> : Specification<DailyHealthLog, T>
{
    public DailyHealthLogByIdSpec(DefaultIdType id)
    {
        Query.Where(p => p.Id == id);
    }
}


public class CurrentDailyHealthLogSpec<T> : Specification<DailyHealthLog, T>
{
    public CurrentDailyHealthLogSpec(DefaultIdType clientId, DateOnly? date)
    {
        Query.Where(p => p.ClientId == clientId)
            .Where(p => p.LogDate == date, date.HasValue).AsNoTrackingWithIdentityResolution().OrderByDescending(x => x.LogDate);
    }
}

public class CurrentWeekDailyHealthLogSpec<T> : Specification<DailyHealthLog, T>
{
    public CurrentWeekDailyHealthLogSpec(DefaultIdType clientId)
    {
        Query.Where(p => p.ClientId == clientId && p.LogDate >= DateOnly.FromDateTime(DateTime.Now.AddDays(-6))).AsNoTrackingWithIdentityResolution().OrderByDescending(x => x.LogDate);
    }
}
