using System.Runtime.CompilerServices;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Core;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain;
using ZeroFat.NutriPlan.Domain.MealPlanning;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.MealPlanning.Meals;
public class UpdateMealRequest : ICommand<Result<Guid>>
{
    public Guid? Id { get; set; }
    public string? NameEn { get; set; } // Name of the meal (e.g., "Grilled Chicken Salad")
    public string? NameAr { get; set; } // Name of the meal (e.g., "Grilled Chicken Salad")
    public bool IsDefault { get; set; }
    public IFormFile? Image { get; set; }

    public double PriceForCustomer { get; set; }
    public bool IsActive { get; set; }
    public bool IsAddOn { get; set; }
    public bool SuitableForFreezing { get; set; }

}

public class UpdateMealRequestValidator : CustomValidator<UpdateMealRequest>
{
    public UpdateMealRequestValidator(
        IReadRepository<Meal> repository,
        IReadRepository<Category> categoryRepo,
        IReadRepository<MealType> mealTypeRepo,
        IStringLocalizer<UpdateMealRequestValidator> localaizer)
    {

    }
}

public class UpdateMealRequestHandler(
    IRepositoryWithEvents<Meal> repository,
    IRepositoryWithEvents<Recipe> recipeRepo,
    IRepositoryWithEvents<IngredientAllergen> ingredientAllergenRepo,
    IRepositoryWithEvents<MealAllergen> mealAllergenRepo,
    IFileStorageManager fileStorageManager,
    IStringLocalizer<UpdateMealRequestHandler> localizer) : IRequestHandler<UpdateMealRequest, Result<Guid>>
{
    private readonly IRepositoryWithEvents<Meal> _repository = repository;
    private readonly IRepositoryWithEvents<Recipe> _recipeRepo = recipeRepo;
    private readonly IRepositoryWithEvents<IngredientAllergen> _ingredientAllergenRepo = ingredientAllergenRepo;
    private readonly IRepositoryWithEvents<MealAllergen> _mealAllergenRepo = mealAllergenRepo;
    private readonly IStringLocalizer<UpdateMealRequestHandler> _localizer = localizer;
    private readonly IFileStorageManager _fileStorageManager = fileStorageManager;

    public async Task<Result<Guid>> Handle(UpdateMealRequest request, CancellationToken cancellationToken)
    {
        Meal? meal = await _repository.GetByIdAsync(request.Id!.Value, cancellationToken);
        _ = meal ?? throw new NotFoundException(_localizer["Meal not found"]);

        Recipe? recipe = await _recipeRepo.GetByIdAsync(meal.RecipeId!.Value, cancellationToken);
        _ = recipe ?? throw new NotFoundException("recipe not found");

        List<MealAllergen> mealAllergens = await _mealAllergenRepo.ListAsync(new ExpressionSpecification<MealAllergen>(x => x.MealId == request.Id), cancellationToken);
        await _mealAllergenRepo.DeleteRangeAsync(mealAllergens, withSaveChanges: false, cancellationToken: cancellationToken);

        List<IngredientAllergen> ingredientAllergens = await _ingredientAllergenRepo.ListAsync(new ExpressionSpecification<IngredientAllergen>(x => x.Ingredient.RecipeIngredients.Any(x => x.RecipeId == meal.RecipeId)), cancellationToken);

        meal.ImageUrl = request.Image == null ? meal.ImageUrl : await _fileStorageManager.UploadAsync<Meal>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
        meal.NameAr = request.NameAr;
        meal.NameEn = request.NameEn;
        meal.Calories = recipe.Calories;
        meal.Protein = recipe.Protein;
        meal.Water = recipe.Water;
        meal.WeightInGrams = recipe.WeightInGrams;
        meal.FullRecipeTextAr = recipe.FullRecipeTextAr;
        meal.FullRecipeTextEn = recipe.FullRecipeTextEn;
        meal.PreparationMethodAr = recipe.PreparationMethodAr;
        meal.PreparationMethodEn = recipe.PreparationMethodEn;

        meal.Carbs = recipe.Carbs;
        meal.Fat = recipe.Fat;
        meal.Cuisine = recipe.Cuisine;
        meal.SuitableForFreezing = request.SuitableForFreezing;
        meal.PriceForCustomer = request.PriceForCustomer;
        meal.IsAddOn = request.IsAddOn;
        meal.IsActive = request.IsActive;
        meal.IsCold = recipe.IsCold;
        meal.IsWarm = recipe.IsWarm;
        meal.IsDairyFree = recipe.IsDairyFree;
        meal.DietaryCategories = recipe.DietaryCategories;
        meal.IsGlutenFree = recipe.IsGlutenFree;
        meal.IsLowGI = recipe.IsLowGI;

        await _mealAllergenRepo.AddRangeAsync(ingredientAllergens.Select(x => x.AllergenId).Distinct().ToList().ConvertAll(x => new MealAllergen() { AllergenId = x, MealId = request.Id }), withSaveChanges: false, cancellationToken: cancellationToken);

        await _repository.UpdateAsync(meal, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<Guid>.SuccessAsync(meal.Id);
    }
}

