using Mapster;
using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Extensions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Application.MeasurementUnits;
using ZeroFat.NutriPlan.Application.Settings.MeasurementUnits;
using ZeroFat.NutriPlan.Domain.MealPlanning;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.MealPlanning.Ingredients;
public class GetIngredientRequest(Guid id) : IQuery<Result<IngredientDetailsDto>>
{
    public Guid Id { get; set; } = id;
}

public class GetIngredientRequestHandler(IReadRepository<Ingredient> repository, IReadRepository<MeasurementUnit> measurementUnitRepo, IStringLocalizer<GetIngredientRequestHandler> localizer) : IRequestHandler<GetIngredientRequest, Result<IngredientDetailsDto>>
{
    private readonly IReadRepository<Ingredient> _repository = repository;
    private readonly IReadRepository<MeasurementUnit> _measurementUnitRepo = measurementUnitRepo;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<IngredientDetailsDto>> Handle(GetIngredientRequest request, CancellationToken cancellationToken)
    {
        TypeAdapterConfig config = new TypeAdapterConfig();
        config.NewConfig<Ingredient, IngredientDetailsDto>()
                .Map(destination => destination.Allergens, src => src.IngredientAllergens.Select(x => x.Allergen));

        var entity = await _repository.FirstOrDefaultAsync(new IngredientByIdSpec<IngredientDetailsDto>(request.Id), config, cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["Ingredient not found", request.Id]);

        if(entity.IngredientMeasurementUnits?.Count > 0)
        {
            foreach(var ingredientMeasurementUnit in entity.IngredientMeasurementUnits)
            {
                if (ingredientMeasurementUnit.Code.HasValue())
                {
                    ingredientMeasurementUnit.MeasurementUnit = await _measurementUnitRepo.FirstOrDefaultAsync(new MeasurementUnitByCodeSpec<MeasurementUnitSimplifyDto>(ingredientMeasurementUnit.Code), cancellationToken);
                }
            }
        }
        return await Result<IngredientDetailsDto>.SuccessAsync(entity);
    }

}
