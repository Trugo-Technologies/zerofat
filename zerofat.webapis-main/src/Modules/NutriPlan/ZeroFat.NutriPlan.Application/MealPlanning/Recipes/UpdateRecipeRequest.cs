using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Extensions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.MealPlanning;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.MealPlanning.Recipes;
public class UpdateRecipeRequest : ICommand<Result<Guid>>
{
    public Guid? Id { get; set; }

    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public string? PreparationMethodEn { get; set; }
    public string? PreparationMethodAr { get; set; }
    public string? FullRecipeTextEn { get; set; }
    public string? FullRecipeTextAr { get; set; }
    public int PreparationTime { get; set; }
    public int CookingTime { get; set; }
    public int Servings { get; set; }
    public RecipeDifficulty Difficulty { get; set; }
    public CuisineType? Cuisine { get; set; }

    public bool IsVegan { get; set; }
    public bool IsVegetarian { get; set; }
    public bool IsGlutenFree { get; set; }
    public bool IsDairyFree { get; set; }
    public bool IsLowGI { get; set; }
    public string? ImageUrl { get; set; }

    public bool IsCold { get; set; }
    public bool IsWarm { get; set; }

    public List<string> Tags { get; set; } = [];

    public DefaultIdType? CategoryId { get; set; }
    public List<DietaryCategory> DietaryCategories { get; set; } = [];
    public List<Guid> RecipeMealTypeIds { get; set; } = [];
    public List<RecipeIngredientRequest> RecipeIngredients { get; set; } = [];
}

public class UpdateRecipeRequestValidator : CustomValidator<UpdateRecipeRequest>
{
    public UpdateRecipeRequestValidator(
        IReadRepository<Recipe> repository,
        IReadRepository<Category> categoryRepo,
        IReadRepository<MealType> mealTypeRepo,
        IStringLocalizer<UpdateRecipeRequestValidator> localaizer)
    {
        RuleFor(u => u.CategoryId)
          .Cascade(CascadeMode.Stop)
          .NotEmpty()
          .MustAsync(async (id, _) => await categoryRepo.AnyAsync(new ExpressionSpecification<Category>(x => x.Id == id && x.CategoryType == CategoryType.Recipe), _))
               .WithMessage(localaizer["Category not found"]);

        When(x => x.RecipeMealTypeIds != null, () =>
        {
            RuleForEach(u => u.RecipeMealTypeIds)
              .Cascade(CascadeMode.Stop)
              .NotEmpty()
              .MustAsync(async (id, _) => await mealTypeRepo.AnyAsync(new ExpressionSpecification<MealType>(x => x.Id == id), _))
                   .WithMessage(localaizer["one MealType not found"]);
        });
    }
}

public class UpdateRecipeRequestHandler(
    IRepositoryWithEvents<Recipe> repository,
    IRepositoryWithEvents<Ingredient> ingredientRepo,
    IRepositoryWithEvents<IngredientMeasurementUnit> ingredientMeasurementUnitRepo,
    IRepositoryWithEvents<RecipeIngredient> recipeIngredientRepo,
    IRepositoryWithEvents<Meal> mealRepo,
    IFileStorageManager fileStorageManager,
    IRepositoryWithEvents<RecipeMealType> recipeMealTypeRepo,
    IStringLocalizer<UpdateRecipeRequestHandler> localizer) : IRequestHandler<UpdateRecipeRequest, Result<Guid>>
{
    private readonly IRepositoryWithEvents<Recipe> _repository = repository;
    private readonly IRepositoryWithEvents<Meal> _mealRepo = mealRepo;
    private readonly IRepositoryWithEvents<IngredientMeasurementUnit> _ingredientMeasurementUnitRepo = ingredientMeasurementUnitRepo;
    private readonly IRepositoryWithEvents<Ingredient> _ingredientRepo = ingredientRepo;
    private readonly IRepositoryWithEvents<RecipeIngredient> _recipeIngredientRepo = recipeIngredientRepo;
    private readonly IRepositoryWithEvents<RecipeMealType> _recipeMealTypeRepo = recipeMealTypeRepo;
    private readonly IStringLocalizer<UpdateRecipeRequestHandler> _localizer = localizer;
    private readonly IFileStorageManager _fileStorageManager = fileStorageManager;

    public async Task<Result<Guid>> Handle(UpdateRecipeRequest request, CancellationToken cancellationToken)
    {
        var recipe = await _repository.GetByIdAsync(request.Id!.Value, cancellationToken);

        _ = recipe ?? throw new NotFoundException(_localizer["Recipe not found"]);

        recipe.NameEn = request.NameEn;
        recipe.NameAr = request.NameAr;
        recipe.CategoryId = request.CategoryId;

        recipe.CookingTime = request.CookingTime;
        recipe.PreparationTime = request.PreparationTime;
        recipe.Servings = request.Servings;
        recipe.Cuisine = request.Cuisine;
        recipe.Difficulty = request.Difficulty;
        recipe.Tags = request.Tags;
        recipe.DietaryCategories = request.DietaryCategories;
        recipe.PreparationMethodEn = request.PreparationMethodEn;
        recipe.PreparationMethodAr = request.PreparationMethodAr;

        recipe.FullRecipeTextEn = request.FullRecipeTextEn;
        recipe.FullRecipeTextAr = request.FullRecipeTextAr;

        recipe.IsWarm = request.IsWarm;
        recipe.IsGlutenFree = request.IsGlutenFree;
        recipe.IsLowGI = request.IsLowGI;
        recipe.IsCold = request.IsCold;
        recipe.IsDairyFree = request.IsDairyFree;


        if (request.RecipeMealTypeIds?.Count > 0)
        {
            await _recipeMealTypeRepo.SyncRelationAsync(
                   new ExpressionSpecification<RecipeMealType>(x => x.RecipeId == recipe.Id),
                   request.RecipeMealTypeIds,
                   x => x.MealTypeId.Value,
                   id => new RecipeMealType { RecipeId = recipe.Id, MealTypeId = id },
                   cancellationToken);

        }

        recipe.WeightInGrams = 0;
        recipe.Protein = 0;
        recipe.Water = 0;
        recipe.Calories = 0;
        recipe.Carbs = 0;
        recipe.Fat = 0;

        if (request.RecipeIngredients?.Count > 0)
        {
            List<RecipeIngredient> recipeIngredients = await _recipeIngredientRepo.ListAsync(new ExpressionSpecification<RecipeIngredient>(x => x.RecipeId == recipe.Id), cancellationToken);
            await _recipeIngredientRepo.DeleteRangeAsync(recipeIngredients, withSaveChanges: false, cancellationToken: cancellationToken);


            foreach (var recipeIngredientModel in request.RecipeIngredients)
            {
                var ingredient = await _ingredientRepo.GetByIdAsync(recipeIngredientModel.IngredientId, cancellationToken);
                var ingredientUnitMeasurement = await _ingredientMeasurementUnitRepo.FirstOrDefaultAsync(new ExpressionSpecification<IngredientMeasurementUnit>(x => x.IngredientId == recipeIngredientModel.IngredientId && x.Code == recipeIngredientModel.MeasurementUnitCode), cancellationToken);
                var recipeIngredient = new RecipeIngredient
                {
                    IngredientId = recipeIngredientModel.IngredientId,
                    RecipeId = recipe.Id,
                    MeasurementUnitCode = recipeIngredientModel.MeasurementUnitCode,
                    Amount = recipeIngredientModel.Amount,
                    IsOptional = recipeIngredientModel.IsOptional,
                    BasicUnit = ingredient.BasicUnit == BasicUnitType.Liquid ? BasicUnit.ml : BasicUnit.g,
                    HideOnCustomerPorter = recipeIngredientModel.HideOnCustomerPorter,
                };

                recipeIngredient.WeightInGrams = recipeIngredientModel.Amount;
                if (ingredientUnitMeasurement != null)
                    recipeIngredient.WeightInGrams *= ingredientUnitMeasurement.EquivalentInUnit;

                if (ingredient.BasicUnit == BasicUnitType.Liquid)
                    recipeIngredient.WeightInGrams *= ingredient.Density;

                recipe.WeightInGrams += recipeIngredient.WeightInGrams;
                recipe.Protein += ingredient.ProteinPer100Unit * recipeIngredient.WeightInGrams / 100;
                recipe.Water += ingredient.WaterPer100g * recipeIngredient.WeightInGrams / 100;
                recipe.Calories += ingredient.CaloriesPer100Unit * recipeIngredient.WeightInGrams / 100;
                recipe.Carbs += ingredient.CarbsPer100Unit * recipeIngredient.WeightInGrams / 100;
                recipe.Fat += ingredient.FatPer100Unit * recipeIngredient.WeightInGrams / 100;

                await _recipeIngredientRepo.AddAsync(recipeIngredient, withSaveChanges: false, cancellationToken: cancellationToken);
            }

        }

        var meals = await _mealRepo.ListAsync(new ExpressionSpecification<Meal>(x => x.RecipeId == recipe.Id), cancellationToken);
        foreach (var meal in meals)
        {
            meal.Calories = recipe.Calories;
            meal.Protein = recipe.Protein;
            meal.Water = recipe.Water;
            meal.WeightInGrams = recipe.WeightInGrams;
            meal.Carbs = recipe.Carbs;
            meal.Fat = recipe.Fat;
            meal.Cuisine = recipe.Cuisine;
            meal.IsCold = recipe.IsCold;
            meal.IsWarm = recipe.IsWarm;
            meal.IsDairyFree = recipe.IsDairyFree;
            meal.DietaryCategories = recipe.DietaryCategories;
            meal.IsGlutenFree = recipe.IsGlutenFree;
            meal.IsLowGI = recipe.IsLowGI;
            meal.FullRecipeTextEn = recipe.FullRecipeTextEn;
            meal.FullRecipeTextAr = recipe.FullRecipeTextAr;
            meal.PreparationMethodEn = recipe.PreparationMethodEn;
            meal.PreparationMethodAr = recipe.PreparationMethodAr;

        }

        await _repository.UpdateAsync(recipe, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<Guid>.SuccessAsync(recipe.Id);
    }
}

