namespace ZeroFat.ClientPortal.Domain.Common.ValueObjects;

public record BodyMeasurement
{
    public double Value { get; init; }
    public HealthMeasurementUnit Unit { get; init; }

    // Parameterless constructor for EF Core
    private BodyMeasurement()
    {
        Value = 0;
        Unit = HealthMeasurementUnit.Kilograms; // Default value
    }

    // Primary constructor
    public BodyMeasurement(double value, HealthMeasurementUnit unit)
    {
        Value = value;
        Unit = unit;
    }

    public double InMetricUnits => Unit.IsWeightUnit
        ? MeasurementConverter.Convert(Value, Unit, HealthMeasurementUnit.Kilograms)
        : MeasurementConverter.Convert(Value, Unit, HealthMeasurementUnit.Centimeters);

    public double InImperialUnits => Unit.IsWeightUnit
        ? MeasurementConverter.Convert(Value, Unit, HealthMeasurementUnit.Pounds)
        : MeasurementConverter.Convert(Value, Unit, HealthMeasurementUnit.Inches);

    public BodyMeasurement ConvertTo(HealthMeasurementUnit targetUnit) =>
        new(MeasurementConverter.Convert(Value, Unit, targetUnit), targetUnit);

    public string DisplayString => $"{Value:0.##} {Unit.Symbol}";
}

