using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Enums;

namespace ZeroFat.Application.Core.Contracts;

public interface IZeroFatService : ITransientService
{
    double CalculateClientBMR(double weight, double height, DateTime birthDate, Gender gender);

    double CalculateBodyFatBasedOnBMI(double bmi, DateTime birthDate, Gender gender);

    double CalculateClientTDEE(double bmr, double physicalActivity);

    double CalculateClientBMI(double weight, double height);

    (int timeNeededInWeeks, double totalCaloriesNeeded) CalculateClientDailyCalories(
     double weight,
     double targetWeight,
     double tDEE,
     NutriPlanStartegy strategy,
     double deficit = 0,
     double surplus = 0,
     int timeInWeeks = 0);
}


