using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.MealPlanning;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.MealPlanning.Recipes;
public class CreateRecipeRequest : ICommand<Result<DefaultIdType>>
{
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

public class RecipeIngredientRequest
{
    public bool HideOnCustomerPorter { get; set; }
    public bool IsOptional { get; set; }
    public double Amount { get; set; }
    public DefaultIdType IngredientId { get; set; }
    public string? MeasurementUnitCode { get; set; }
}

public class CreateRecipeRequestValidator : CustomValidator<CreateRecipeRequest>
{
    public CreateRecipeRequestValidator(
        IReadRepository<Recipe> repository,
        IReadRepository<Category> categoryRepo,
        IReadRepository<MealType> mealTypeRepo,
        IStringLocalizer<CreateRecipeRequestValidator> localaizer)
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


public class CreateRecipeRequestHandler(
    IRepositoryWithEvents<Recipe> repository,
    IRepositoryWithEvents<Ingredient> ingredientRepo,
    IRepositoryWithEvents<IngredientMeasurementUnit> ingredientMeasurementUnitRepo,
    IFileStorageManager fileStorageManager) : ICommandHandler<CreateRecipeRequest, Result<Guid>>
{
    private readonly IRepositoryWithEvents<Recipe> _repository = repository;
    private readonly IRepositoryWithEvents<Ingredient> _ingredientRepo = ingredientRepo;
    private readonly IRepositoryWithEvents<IngredientMeasurementUnit> _ingredientMeasurementUnitRepo = ingredientMeasurementUnitRepo;
    private readonly IFileStorageManager _fileStorageManager = fileStorageManager;

    public async Task<Result<DefaultIdType>> Handle(CreateRecipeRequest request, CancellationToken cancellationToken)
    {
        var recipe = new Recipe
        {
            NameEn = request.NameEn,
            NameAr = request.NameAr,
            CategoryId = request.CategoryId,

            CookingTime = request.CookingTime,
            PreparationTime = request.PreparationTime,
            Servings = request.Servings,
            Cuisine = request.Cuisine,
            Difficulty = request.Difficulty,
            Tags = request.Tags,
            DietaryCategories = request.DietaryCategories,
            PreparationMethodEn = request.PreparationMethodEn,
            PreparationMethodAr = request.PreparationMethodAr,
            FullRecipeTextAr = request.FullRecipeTextAr,
            FullRecipeTextEn = request.FullRecipeTextEn,

            IsWarm = request.IsWarm,
            IsGlutenFree = request.IsGlutenFree,
            IsLowGI = request.IsLowGI,
            IsCold = request.IsCold,
            IsDairyFree = request.IsDairyFree,

            RecipeMealTypes = request.RecipeMealTypeIds.ConvertAll(id => new RecipeMealType { MealTypeId = id })
        };

        if (request.RecipeIngredients.Count > 0)
        {
            foreach (var recipeIngredientModel in request.RecipeIngredients)
            {
                var ingredient = await _ingredientRepo.GetByIdAsync(recipeIngredientModel.IngredientId, cancellationToken);
                var ingredientUnitMeasurement = await _ingredientMeasurementUnitRepo.FirstOrDefaultAsync(new ExpressionSpecification<IngredientMeasurementUnit>(x => x.IngredientId == recipeIngredientModel.IngredientId && x.Code == recipeIngredientModel.MeasurementUnitCode), cancellationToken);
                
                var recipeIngredient = new RecipeIngredient
                {
                    IngredientId = recipeIngredientModel.IngredientId,
                    MeasurementUnitCode = recipeIngredientModel.MeasurementUnitCode,
                    Amount = recipeIngredientModel.Amount,
                    IsOptional = recipeIngredientModel.IsOptional,
                    BasicUnit = ingredient.BasicUnit == BasicUnitType.Liquid ? BasicUnit.ml : BasicUnit.g,
                    HideOnCustomerPorter = recipeIngredientModel.HideOnCustomerPorter,
                };

                recipeIngredient.WeightInGrams = recipeIngredientModel.Amount;

                if (ingredientUnitMeasurement != null)
                {
                    recipeIngredient.WeightInGrams *= ingredientUnitMeasurement.EquivalentInUnit;
                }

                if (ingredient.BasicUnit == BasicUnitType.Liquid)
                {
                    recipeIngredient.WeightInGrams *= ingredient.Density;
                }

                recipe.WeightInGrams += recipeIngredient.WeightInGrams;
                recipe.Protein += ingredient.ProteinPer100Unit * recipeIngredient.WeightInGrams / 100;
                recipe.Water += ingredient.WaterPer100g * recipeIngredient.WeightInGrams / 100;
                recipe.Calories += ingredient.CaloriesPer100Unit * recipeIngredient.WeightInGrams / 100;
                recipe.Carbs += ingredient.CarbsPer100Unit * recipeIngredient.WeightInGrams / 100;
                recipe.Fat += ingredient.FatPer100Unit * recipeIngredient.WeightInGrams / 100;

                recipe.RecipeIngredients.Add(recipeIngredient);



               // recipe.Ingredient.CostPer100Unit *= 100;
               // if (recipeIngredien.Ingredient.BasicUnit == BasicUnitType.Solid)
               // {
               //     recipeIngredien.BasicUnit = BasicUnit.g;
               //     recipeIngredien.MeasurementUnitCode = "G";
               // }
               // else
               // {
               //     recipeIngredien.BasicUnit = BasicUnit.ml;
               //     recipeIngredien.MeasurementUnitCode = "ML";
               // }

            }

        }

        await _repository.AddAsync(recipe, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(recipe.Id);
    }

}
