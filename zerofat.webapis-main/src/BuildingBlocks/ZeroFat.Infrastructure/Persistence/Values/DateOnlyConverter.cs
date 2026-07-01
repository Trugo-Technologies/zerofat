using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ZeroFat.Infrastructure.Persistence.Values;

public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
{
    public DateOnlyConverter()
        : base(
            dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
            dateTime => DateOnly.FromDateTime(dateTime))
    {
    }
}
