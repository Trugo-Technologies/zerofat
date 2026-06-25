using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ZeroFat.Infrastructure.Persistence.Values;

public class JsonValueConverter<T> : ValueConverter<T, string?>
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    public JsonValueConverter()
        : base(
            v => JsonSerializer.Serialize(v, Options),
            v => string.IsNullOrWhiteSpace(v)
                ? default!
                : JsonSerializer.Deserialize<T>(v, Options)!)
    {
    }
}
