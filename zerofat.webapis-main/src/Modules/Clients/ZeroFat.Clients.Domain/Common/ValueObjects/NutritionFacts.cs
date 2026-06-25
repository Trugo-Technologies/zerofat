namespace ZeroFat.ClientPortal.Domain.Common.ValueObjects;

public sealed record NutritionFacts(
    double Calories,
    double ProteinInGrams,
    double CarbsInGrams,
    double FatInGrams);
