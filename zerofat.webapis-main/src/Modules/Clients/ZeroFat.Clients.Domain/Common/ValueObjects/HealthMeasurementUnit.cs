namespace ZeroFat.ClientPortal.Domain.Common.ValueObjects;
public record HealthMeasurementUnit
{
    public string Symbol { get; init; }

    // Parameterless constructor for EF Core
    private HealthMeasurementUnit()
    {
        Symbol = "kg";
    }

    // Primary constructor
    public HealthMeasurementUnit(string symbol)
    {
        Symbol = symbol;
    }

    // Weight units
    public static readonly HealthMeasurementUnit Kilograms = new("kg");
    public static readonly HealthMeasurementUnit Pounds = new("lb");
    public static readonly HealthMeasurementUnit Grams = new("g");

    // Height/Length units
    public static readonly HealthMeasurementUnit Centimeters = new("cm");
    public static readonly HealthMeasurementUnit Inches = new("in");
    public static readonly HealthMeasurementUnit Feet = new("ft");
    public static readonly HealthMeasurementUnit Meters = new("m");

    // Volume units
    public static readonly HealthMeasurementUnit Liters = new("L");
    public static readonly HealthMeasurementUnit FluidOunces = new("fl oz");
    public static readonly HealthMeasurementUnit Milliliters = new("ml");

    // Conversion helper methods
    public double ConvertTo(double value, HealthMeasurementUnit targetUnit) =>
        MeasurementConverter.Convert(value, this, targetUnit);

    public bool IsWeightUnit => this == Kilograms || this == Pounds || this == Grams;
    public bool IsHeightUnit => this == Centimeters || this == Inches || this == Feet || this == Meters;
    public bool IsVolumeUnit => this == Liters || this == FluidOunces || this == Milliliters;
}

public static class MeasurementConverter
{
    // Weight conversions
    public static double PoundsToKilograms(double pounds) => pounds * 0.45359237;
    public static double KilogramsToPounds(double kg) => kg / 0.45359237;

    // Height conversions
    public static double InchesToCentimeters(double inches) => inches * 2.54;
    public static double CentimetersToInches(double cm) => cm / 2.54;
    public static double FeetToInches(double feet) => feet * 12;
    public static double InchesToFeet(double inches) => inches / 12;
    public static double FeetToCentimeters(double feet) => InchesToCentimeters(FeetToInches(feet));
    public static double CentimetersToFeet(double cm) => InchesToFeet(CentimetersToInches(cm));

    // Volume conversions
    public static double FluidOuncesToLiters(double flOz) => flOz * 0.0295735;
    public static double LitersToFluidOunces(double liters) => liters / 0.0295735;

    // Unified conversion method
    public static double Convert(double value, HealthMeasurementUnit fromUnit, HealthMeasurementUnit toUnit)
    {
        if (fromUnit == toUnit) return value;

        return (fromUnit.Symbol, toUnit.Symbol) switch
        {
            // Weight conversions
            ("lb", "kg") => value * 0.45359237,
            ("kg", "lb") => value / 0.45359237,
            ("lb", "g") => value * 453.59237,
            ("kg", "g") => value * 1000,
            ("g", "kg") => value / 1000,
            ("g", "lb") => value / 453.59237,

            // Height conversions
            ("in", "cm") => value * 2.54,
            ("cm", "in") => value / 2.54,
            ("ft", "in") => value * 12,
            ("in", "ft") => value / 12,
            ("ft", "cm") => value * 30.48,
            ("cm", "ft") => value / 30.48,
            ("m", "cm") => value * 100,
            ("cm", "m") => value / 100,
            ("m", "in") => value * 39.3701,
            ("in", "m") => value / 39.3701,
            ("m", "ft") => value * 3.28084,
            ("ft", "m") => value / 3.28084,

            // Volume conversions
            ("fl oz", "L") => value * 0.0295735,
            ("L", "fl oz") => value / 0.0295735,
            ("ml", "L") => value / 1000,
            ("L", "ml") => value * 1000,
            ("fl oz", "ml") => value * 29.5735,
            ("ml", "fl oz") => value / 29.5735,

            _ => throw new InvalidOperationException($"Conversion from {fromUnit.Symbol} to {toUnit.Symbol} is not supported")
        };
    }
}
