using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ZeroFat.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TrailType
{
    None = 0,
    Create = 1,
    Update = 2,
    Delete = 3
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FileType
{
    [Description(".jpg,.png,.jpeg,.gif")]
    Image,
    [Description(".pdf")]
    PdfFile,
    Other
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ApplicationModule
{
    NutriPlan,
    Workout,
    ClientPortal,

    UsersModule = 10,
    AuditingModule = 99
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PropertyType
{
    Number,
    String,
    Text,
    Boolean,
    Date,
    Decimal,
    Array,
    Object,
}
