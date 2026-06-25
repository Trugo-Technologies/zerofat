using Mapster;
using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.Meals;
public class GetMealRequest(Guid id) : IQuery<Result<MealDetailsDto>>
{
    public Guid Id { get; set; } = id;
}

public class GetMealRequestHandler(IReadRepository<Meal> repository, IStringLocalizer<GetMealRequestHandler> localizer) : IRequestHandler<GetMealRequest, Result<MealDetailsDto>>
{
    private readonly IReadRepository<Meal> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<MealDetailsDto>> Handle(GetMealRequest request, CancellationToken cancellationToken)
    {
        TypeAdapterConfig config = new();
        config.NewConfig<Meal, MealDetailsDto>()
                .Map(destination => destination.Allergens, src => src.Allergens.Select(x => x.Allergen));

        var entity = await _repository.FirstOrDefaultAsync(new MealByIdSpec<MealDetailsDto>(request.Id), config, cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["Meal not found", request.Id]);

        return await Result<MealDetailsDto>.SuccessAsync(entity);
    }

}
