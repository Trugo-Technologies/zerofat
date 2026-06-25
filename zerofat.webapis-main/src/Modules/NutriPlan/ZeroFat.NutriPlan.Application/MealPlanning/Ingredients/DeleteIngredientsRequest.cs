using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.Ingredients;
public class DeleteIngredientsRequest : ICommand<Result<bool>>
{
    public List<DefaultIdType> Ids { get; set; } = [];
}

public class DeleteIngredientsRequestHandler(
    IRepositoryWithEvents<Ingredient> repository,
    IRepositoryWithEvents<IngredientAllergen> ingredientAllergenRepo,
    IRepositoryWithEvents<IngredientMeasurementUnit> ingredientMeasurementUnitRepo,
    IRepositoryWithEvents<RecipeIngredient> recipeIngredientRepo,
    IRepositoryWithEvents<IngredientAttribute> ingredientAttributeRepo,
    IStringLocalizer<DeleteIngredientsRequestHandler> localizer) : 
    IRequestHandler<DeleteIngredientsRequest, Result<bool>>
{
    private readonly IRepositoryWithEvents<Ingredient> _repository = repository;
    private readonly IRepositoryWithEvents<IngredientAllergen> _ingredientAllergenRepo = ingredientAllergenRepo;
    private readonly IRepositoryWithEvents<IngredientMeasurementUnit> _ingredientMeasurementUnitRepo = ingredientMeasurementUnitRepo;
    private readonly IRepositoryWithEvents<RecipeIngredient> _recipeIngredientRepo = recipeIngredientRepo;
    private readonly IRepositoryWithEvents<IngredientAttribute> _ingredientAttributeRepo = ingredientAttributeRepo;

    public async Task<Result<bool>> Handle(DeleteIngredientsRequest request, CancellationToken cancellationToken)
    {
        foreach(var ingId in request.Ids)
        {
            var ingredient = await _repository.GetByIdAsync(ingId, cancellationToken);
            if(ingredient != null)
            {
                var hasRecipeIngrediens = await _recipeIngredientRepo.AnyAsync(new ExpressionSpecification<RecipeIngredient>(x => x.IngredientId == ingId), cancellationToken: cancellationToken);
                if (!hasRecipeIngrediens)
                {
                    List<IngredientAllergen> ingredientAllergens = await _ingredientAllergenRepo.ListAsync(new ExpressionSpecification<IngredientAllergen>(x => x.IngredientId == ingId), cancellationToken: cancellationToken);
                    await _ingredientAllergenRepo.DeleteRangeAsync(ingredientAllergens, withSaveChanges: false, cancellationToken: cancellationToken);

                    List<IngredientMeasurementUnit> ingredientMeasurementUnits = await _ingredientMeasurementUnitRepo.ListAsync(new ExpressionSpecification<IngredientMeasurementUnit>(x => x.IngredientId == ingId), cancellationToken: cancellationToken);
                    await _ingredientMeasurementUnitRepo.DeleteRangeAsync(ingredientMeasurementUnits, withSaveChanges: false, cancellationToken: cancellationToken);

                    List<IngredientAttribute> ingredientAttributes = await _ingredientAttributeRepo.ListAsync(new ExpressionSpecification<IngredientAttribute>(x => x.IngredientId == ingId), cancellationToken: cancellationToken);
                    await _ingredientAttributeRepo.DeleteRangeAsync(ingredientAttributes, withSaveChanges: false, cancellationToken: cancellationToken);

                    await _repository.DeleteAsync(ingredient, withSaveChanges: false, cancellationToken: cancellationToken);
                }
            }
        }

        await _repository.SaveChangesAsync(cancellationToken);

        return await Result<bool>.SuccessAsync(data :true);
    }

}
