
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.Contracts;

public interface INutriPlanExcelReader : ITransientService
{
    List<Ingredient> ReadIngredientsFromExcel(string filePath);
}
