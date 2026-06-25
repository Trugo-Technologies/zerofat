using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.Recipes;

public class DeleteRecipeRequest : ICommand<Result<Guid>>
{
    public Guid Id { get; set; }
    public DeleteRecipeRequest(Guid id) => Id = id;
}


public class DeleteRecipeRequestHandler(
    IRepositoryWithEvents<Recipe> repository,
    IRepositoryWithEvents<RecipeIngredient> recipeIngredientRepo,
    IRepositoryWithEvents<Meal> mealRepo,
    IStringLocalizer<DeleteRecipeRequestHandler> localizer) : IRequestHandler<DeleteRecipeRequest, Result<Guid>>
{

    public async Task<Result<Guid>> Handle(DeleteRecipeRequest request, CancellationToken cancellationToken)
    {
        var recipe = await repository.GetByIdAsync(request.Id, cancellationToken);

        _ = recipe ?? throw new NotFoundException(localizer["Recipe not found"]);

        var hasMeal = await mealRepo.AnyAsync(new ExpressionSpecification<Meal>(x => x.RecipeId == request.Id), cancellationToken: cancellationToken);
        if (hasMeal)
        {
            throw new BadRequestException(localizer["Recipe used in other meals"]);
        }

        var recipeIngrediens = await recipeIngredientRepo.ListAsync(new ExpressionSpecification<RecipeIngredient>(x => x.RecipeId == request.Id), cancellationToken: cancellationToken);
        await recipeIngredientRepo.DeleteRangeAsync(recipeIngrediens, withSaveChanges: false, cancellationToken: cancellationToken);

        await repository.DeleteAsync(recipe, cancellationToken);

        return await Result<Guid>.SuccessAsync(recipe.Id);
    }

}
