using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ZeroFat.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DietitianGoal
{
    [Description("المحافظة على الصحة")]
    GeneralHealth,
    [Description("خسارة وزن")]
    LoseWeight,
    [Description("زيادة وزن")]
    GainWeight,
    [Description("حرق دهون")]
    LoseFat,
    [Description("بناء عضلات")]
    BuildMuscles
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum HeightMeasurement
{
    Cm,
    Ft
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WeightMeasurement
{
    Kg,
    Lb
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Gender
{
    [Description("ذكر")]
    Male,
    [Description("انثى")]
    Female
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TransactionType
{
    Earn,
    Consume
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AddressType
{
    [Description("المنزل")]
    Home,
    [Description("المكتب")]
    Office
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StatisticsType
{
    W,
    M,
    M3,
    M6,
    Y
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DiscountDuration
{
    Once,
    Repeating,
    Forever
}
