using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.RecipeIngredients;

public class DeleteRecipeIngredientRequest : ICommand<Result<Guid>>
{
    public Guid Id { get; set; }
    public DeleteRecipeIngredientRequest(Guid id) => Id = id;
}


public class DeleteRecipeIngredientRequestHandler(IRepositoryWithEvents<RecipeIngredient> repository, IStringLocalizer<DeleteRecipeIngredientRequestHandler> localizer) : IRequestHandler<DeleteRecipeIngredientRequest, Result<Guid>>
{
    private readonly IRepositoryWithEvents<RecipeIngredient> _repository = repository;
    private readonly IStringLocalizer<DeleteRecipeIngredientRequestHandler> _localizer = localizer;

    public async Task<Result<Guid>> Handle(DeleteRecipeIngredientRequest request, CancellationToken cancellationToken)
    {
        var RecipeIngredient = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = RecipeIngredient ?? throw new NotFoundException(_localizer["ingredient MeasurementUnit not found"]);

        await _repository.DeleteAsync(RecipeIngredient, cancellationToken);

        return await Result<Guid>.SuccessAsync(RecipeIngredient.Id);
    }

}
