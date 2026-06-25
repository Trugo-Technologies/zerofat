using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Application.MealPlanning.Ingredients;

namespace ZeroFat.NutriPlan.Application.MealPlanning.RecipeIngredients;

public class RecipeIngredientDto : IDto
{
    public DefaultIdType Id { get; set; }
    public bool HideOnCustomerPorter { get; set; }
    public double Amount { get; set; }
    public double WeightInGrams { get; set; }
    public bool IsOptional { get; set; }
    public BasicUnit BasicUnit { get; set; }
    public IngredientSimplifyDto? Ingredient { get; set; }
    public DefaultIdType? IngredientId { get; set; }
    public string? MeasurementUnitCode { get; set; }
}

