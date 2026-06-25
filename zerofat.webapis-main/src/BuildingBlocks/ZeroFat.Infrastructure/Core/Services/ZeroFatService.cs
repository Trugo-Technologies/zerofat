using ZeroFat.Application.Core.Contracts;
using ZeroFat.Domain.Enums;

namespace ZeroFat.Infrastructure.Core.Services;

public class ZeroFatService : IZeroFatService
{
    private const double CaloriesPerKg = 7700;
    private const double DailyCaloricChangeFactor = 7;

    public double CalculateClientBMR(double weight, double height, DateTime birthDate, Gender gender)
    {
        int age = CalculateAge(birthDate);
        double bmr = gender == Gender.Male
            ? 88.362 + (13.397 * weight) + (4.799 * height) - (5.677 * age)
            : 447.593 + (9.247 * weight) + (3.098 * height) - (4.330 * age);

        return Math.Round(bmr, 2);
    }

    public double CalculateClientBMI(double weight, double height)
    {
        double bmi = (weight * 10000) / (height * height);
        return Math.Round(bmi, 2);
    }

    public double CalculateBodyFatBasedOnBMI(double bmi, DateTime birthDate, Gender gender)
    {
        int age = CalculateAge(birthDate);
        double bodyFat = gender == Gender.Male
            ? (1.20 * bmi) + (0.23 * age) - 16.2
            : (1.20 * bmi) + (0.23 * age) - 5.4;

        return Math.Round(bodyFat, 2);
    }

    public double CalculateClientTDEE(double bmr, double physicalActivity)
    {
        return Math.Round(bmr * physicalActivity, 2);
    }

    public (int timeNeededInWeeks, double totalCaloriesNeeded) CalculateClientDailyCalories(
    double weight,
    double targetWeight,
    double tDEE,
    NutriPlanStartegy strategy,
    double deficit = 0,
    double surplus = 0,
    int timeInWeeks = 0)
    {
        double weightToChange = weight - targetWeight;

        if (weightToChange is 0)
        {
            return (0, tDEE); // No change needed
        }

        if (strategy == NutriPlanStartegy.BasedOnCalories)
        {
            // Calorie-based strategy
            double dailyCalorieDifference = weightToChange > 0 ? deficit : surplus;
            double totalCaloriesNeeded = weightToChange > 0 ? -deficit : surplus;

            double timeNeededInWeeks = Math.Abs(weightToChange) * (CaloriesPerKg / DailyCaloricChangeFactor) / dailyCalorieDifference;
            // timeNeeded = Math.Abs(weightToChange) / (dailyCalorieDifference / 1100);
            // double timeNeededInWeeks = Math.Abs(weightToChange) / (dailyCalorieDifference / CaloriesPerKg / DailyCaloricChangeFactor);
            return ((int)Math.Ceiling(timeNeededInWeeks), tDEE + totalCaloriesNeeded);
        }
        else
        {
            // Time-based strategy
            double rate = weightToChange / timeInWeeks;
            double dailyCaloricChange = rate * CaloriesPerKg / DailyCaloricChangeFactor;

            double neededCalories = tDEE - dailyCaloricChange;
            return (timeInWeeks, neededCalories);
        }
    }


    public async Task<(int timeNeeded, double totalCaloriesNeeded)> CalculateClientDailyCaloriesBasedOnCalories(double weight, double targetWeight, double deficit, double surplus, double tDEE)
    {
        await Task.Delay(1);
        double weightToChange = weight - targetWeight;
        double timeNeeded;
        double totalCaloriesNeeded;


        double dailyCalorieDifference;
        if (weightToChange > 0)
        {
            dailyCalorieDifference = deficit;
            totalCaloriesNeeded = -deficit;
        }
        else if (weightToChange < 0)
        {
            dailyCalorieDifference = surplus;
            totalCaloriesNeeded = +surplus;
        }
        else
        {
            return (0, tDEE);
        }

        timeNeeded = Math.Abs(weightToChange) / (dailyCalorieDifference / 1100);

        return ((int)Math.Ceiling(timeNeeded), tDEE + totalCaloriesNeeded);
    }

    private int CalculateAge(DateTime birthDate)
    {
        int age = DateTime.Now.Year - birthDate.Year;
        if (birthDate > DateTime.Now.AddYears(-age))
        {
            age--;
        }
        return age;
    }
}
