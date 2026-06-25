using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ZeroFat.Infrastructure.Persistence.Values;

public class StringListConverter : ValueConverter<List<string>?, string?>
{
    public StringListConverter()
        : base(v => string.Join(", ", v!), v => v.Split(',', StringSplitOptions.TrimEntries).ToList())
    {
    }
}
