using System.Text.Json.Serialization;

namespace ZeroFat.Domain.Enums;
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MeasurementSystem
{
    Metric,   // kg, cm
    Imperial  // lbs, feet/inches
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MeasurementType
{
    Weight,
    Height,
    Volume
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CalorieRecordType
{
    Food,      // Positive calorie intake
    Activity   // Negative calorie impact (burned)
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WeightTrendStatus
{
    OnTrack,     // Blue
    OffTrack,    // Red
    Estimated    // Gray
}
