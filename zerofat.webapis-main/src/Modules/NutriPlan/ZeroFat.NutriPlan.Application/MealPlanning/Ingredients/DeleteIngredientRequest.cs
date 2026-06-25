using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.Ingredients;
public class DeleteIngredientRequest : ICommand<Result<Guid>>
{
    public Guid Id { get; set; }
    public DeleteIngredientRequest(Guid id)
    {
        Id = id;
    }
}

public class DeleteIngredientRequestHandler(
    IRepository<Ingredient> repository,
    IRepositoryWithEvents<IngredientAllergen> ingredientAllergenRepo,
    IRepositoryWithEvents<IngredientMeasurementUnit> ingredientMeasurementUnitRepo,
    IRepositoryWithEvents<RecipeIngredient> recipeIngredientRepo,
    IRepositoryWithEvents<IngredientAttribute> ingredientAttributeRepo,
    IStringLocalizer<DeleteIngredientRequestHandler> localizer) : IRequestHandler<DeleteIngredientRequest, Result<Guid>>
{
    private readonly IRepository<Ingredient> _repository = repository;
    private readonly IRepositoryWithEvents<IngredientAllergen> _ingredientAllergenRepo = ingredientAllergenRepo;
    private readonly IRepositoryWithEvents<IngredientMeasurementUnit> _ingredientMeasurementUnitRepo = ingredientMeasurementUnitRepo;
    private readonly IRepositoryWithEvents<RecipeIngredient> _recipeIngredientRepo = recipeIngredientRepo;
    private readonly IRepositoryWithEvents<IngredientAttribute> _ingredientAttributeRepo = ingredientAttributeRepo;
    private readonly IStringLocalizer<DeleteIngredientRequestHandler> _localizer = localizer;

    public async Task<Result<Guid>> Handle(DeleteIngredientRequest request, CancellationToken cancellationToken)
    {
        Ingredient? entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["Ingredient not found"]);

        var hasRecipeIngrediens = await _recipeIngredientRepo.AnyAsync(new ExpressionSpecification<RecipeIngredient>(x => x.IngredientId == request.Id), cancellationToken: cancellationToken);
        if (hasRecipeIngrediens)
        {
            throw new BadRequestException(_localizer["Ingredient used in other recipes"]);
        }

        List<IngredientAllergen> ingredientAllergens = await _ingredientAllergenRepo.ListAsync(new ExpressionSpecification<IngredientAllergen>(x => x.IngredientId == request.Id), cancellationToken: cancellationToken);
        await _ingredientAllergenRepo.DeleteRangeAsync(ingredientAllergens, withSaveChanges: false, cancellationToken: cancellationToken);

        List<IngredientMeasurementUnit> ingredientMeasurementUnits = await _ingredientMeasurementUnitRepo.ListAsync(new ExpressionSpecification<IngredientMeasurementUnit>(x => x.IngredientId == request.Id), cancellationToken: cancellationToken);
        await _ingredientMeasurementUnitRepo.DeleteRangeAsync(ingredientMeasurementUnits, withSaveChanges: false, cancellationToken: cancellationToken);

        List<IngredientAttribute> ingredientAttributes = await _ingredientAttributeRepo.ListAsync(new ExpressionSpecification<IngredientAttribute>(x => x.IngredientId == request.Id), cancellationToken: cancellationToken);
        await _ingredientAttributeRepo.DeleteRangeAsync(ingredientAttributes, withSaveChanges: false, cancellationToken: cancellationToken);

        await _repository.DeleteAsync(entity, cancellationToken);

        return await Result<Guid>.SuccessAsync(entity.Id);
    }

}
