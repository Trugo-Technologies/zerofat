using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain;
using ZeroFat.NutriPlan.Domain.MealPlanning;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.MealPlanning.Meals;
public class CreateMealRequest : ICommand<Result<DefaultIdType>>
{
    public string? NameEn { get; set; } // Name of the meal (e.g., "Grilled Chicken Salad")
    public string? NameAr { get; set; } // Name of the meal (e.g., "Grilled Chicken Salad")
    public IFormFile? Image { get; set; }

    public DefaultIdType? RecipeId { get; set; } // Foreign key to the recipe
    public double PriceForCustomer { get; set; }
    public bool SuitableForFreezing { get; set; }
    public bool IsActive { get; set; }
    public bool IsAddOn { get; set; }
}


public class CreateMealRequestValidator : CustomValidator<CreateMealRequest>
{
    public CreateMealRequestValidator(
        IReadRepository<Meal> repository,
        IReadRepository<Category> categoryRepo,
        IReadRepository<MealType> mealTypeRepo,
        IStringLocalizer<CreateMealRequestValidator> localaizer)
    {
        RuleFor(x => x.RecipeId).NotEmpty().NotNull();
    }
}


public class CreateMealRequestHandler(
    IRepositoryWithEvents<Meal> repository,
    IRepository<IngredientAllergen> ingredientAllergenRepo,
    IRepository<Recipe> recipeRepo,
    IFileStorageManager fileStorageManager) : ICommandHandler<CreateMealRequest, Result<Guid>>
{
    private readonly IRepositoryWithEvents<Meal> _repository = repository;
    private readonly IRepository<Recipe> _recipeRepo = recipeRepo;
    private readonly IRepository<IngredientAllergen> _ingredientAllergenRepo = ingredientAllergenRepo;
    private readonly IFileStorageManager _fileStorageManager = fileStorageManager;

    public async Task<Result<DefaultIdType>> Handle(CreateMealRequest request, CancellationToken cancellationToken)
    {
        Recipe? recipe = await _recipeRepo.GetByIdAsync(request.RecipeId!.Value, cancellationToken);
        _ = recipe ?? throw new NotFoundException("recipe not found");

        List<IngredientAllergen> ingredientAllergens = await _ingredientAllergenRepo.ListAsync(new ExpressionSpecification<IngredientAllergen>(x => x.Ingredient.RecipeIngredients.Any(x => x.RecipeId == request.RecipeId)), cancellationToken);

        Meal entity = new()
        {
            ImageUrl = request.Image == null ? recipe.ImageUrl : await _fileStorageManager.UploadAsync<Meal>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken),
            RecipeId = request.RecipeId,
            NameAr = request.NameAr,
            NameEn = request.NameEn,
            Calories = recipe.Calories,
            Protein = recipe.Protein,
            Water = recipe.Water,
            FullRecipeTextAr = recipe.FullRecipeTextAr,
            FullRecipeTextEn = recipe.FullRecipeTextEn,
            PreparationMethodAr = recipe.PreparationMethodAr,
            PreparationMethodEn = recipe.PreparationMethodEn,
            WeightInGrams = recipe.WeightInGrams,
            Carbs = recipe.Carbs,
            Fat = recipe.Fat,
            Cuisine = recipe.Cuisine,
            SuitableForFreezing = request.SuitableForFreezing,
            PriceForCustomer = request.PriceForCustomer,
            IsAddOn = request.IsAddOn,
            IsActive = request.IsActive,
            IsCold = recipe.IsCold,
            IsWarm = recipe.IsWarm,
            IsDairyFree = recipe.IsDairyFree,
            DietaryCategories = recipe.DietaryCategories,
            IsGlutenFree = recipe.IsGlutenFree,
            IsLowGI = recipe.IsLowGI,
            Allergens = ingredientAllergens.Select(x => x.AllergenId).Distinct().ToList().ConvertAll(x => new MealAllergen() { AllergenId = x })
        };
        await _repository.AddAsync(entity, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(entity.Id);
    }

}
