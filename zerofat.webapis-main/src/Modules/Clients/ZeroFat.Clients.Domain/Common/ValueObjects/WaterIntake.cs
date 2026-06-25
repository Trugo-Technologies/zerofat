namespace ZeroFat.ClientPortal.Domain.Common.ValueObjects;

public sealed record WaterIntake(double Liters)
{
    public static readonly WaterIntake Zero = new(0);

    public double InFluidOunces => Liters / 0.0295735;
    public double InMilliliters => Liters * 1000;

    public static WaterIntake FromImperial(double fluidOunces) => new(fluidOunces * 0.0295735);
}
