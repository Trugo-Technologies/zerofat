using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ZeroFat.Infrastructure.Persistence.Values;

public class TimeOnlyComparer : ValueComparer<TimeOnly>
{
    public TimeOnlyComparer()
        : base((t1, t2) => t1.Ticks == t2.Ticks, t => t.GetHashCode())
    {
    }
}
