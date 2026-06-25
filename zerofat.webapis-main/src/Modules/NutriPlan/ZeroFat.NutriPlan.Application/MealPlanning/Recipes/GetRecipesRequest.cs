using Mapster;
using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.Recipes;
public class GetRecipeRequest(Guid id) : IQuery<Result<RecipeDetailsDto>>
{
    public Guid Id { get; set; } = id;
}

public class GetRecipeRequestHandler(IReadRepository<Recipe> repository, IStringLocalizer<GetRecipeRequestHandler> localizer) : IRequestHandler<GetRecipeRequest, Result<RecipeDetailsDto>>
{
    private readonly IReadRepository<Recipe> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<RecipeDetailsDto>> Handle(GetRecipeRequest request, CancellationToken cancellationToken)
    {
        TypeAdapterConfig config = new TypeAdapterConfig();
        config.NewConfig<Recipe, RecipeDetailsDto>()
                .Map(destination => destination.MealTypes, src => src.RecipeMealTypes.Select(x => x.MealType));

        var entity = await _repository.FirstOrDefaultAsync(new RecipeByIdSpec<RecipeDetailsDto>(request.Id), config, cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["Recipe not found", request.Id]);

        return await Result<RecipeDetailsDto>.SuccessAsync(entity);
    }

}
