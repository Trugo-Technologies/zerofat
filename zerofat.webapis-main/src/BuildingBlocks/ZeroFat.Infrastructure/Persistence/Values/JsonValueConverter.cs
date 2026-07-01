using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ZeroFat.Infrastructure.Persistence.Values;

public class JsonValueConverter<T> : ValueConverter<T, string?>
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    public JsonValueConverter()
        : base(
            v => JsonSerializer.Serialize(v, Options),
            v => Deserialize(v))
    {
    }

    private static T Deserialize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Trim().Equals("null", StringComparison.OrdinalIgnoreCase))
        {
            return CreateDefault();
        }

        try
        {
            using var document = JsonDocument.Parse(value);
            if (document.RootElement.ValueKind == JsonValueKind.Array)
            {
                return JsonSerializer.Deserialize<T>(value, Options) ?? CreateDefault();
            }

            // Legacy rows may store "" or {} in jsonb instead of [].
            if (IsListType(typeof(T)))
            {
                return CreateDefault();
            }

            return JsonSerializer.Deserialize<T>(value, Options) ?? CreateDefault();
        }
        catch (JsonException)
        {
            return CreateDefault();
        }
    }

    private static bool IsListType(Type type) =>
        type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);

    private static T CreateDefault()
    {
        var type = typeof(T);
        if (IsListType(type))
        {
            var listType = typeof(List<>).MakeGenericType(type.GetGenericArguments()[0]);
            return (T)Activator.CreateInstance(listType)!;
        }

        return default!;
    }
}
