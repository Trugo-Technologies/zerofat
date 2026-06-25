using Mapster;
using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.MenuPlanning;

namespace ZeroFat.NutriPlan.Application.MenuPlanning.DailyMenuMeals;
public class GetDailyMenuMealRequest(Guid id) : IQuery<Result<DailyMenuMealDetailsDto>>
{
    public Guid Id { get; set; } = id;
}

public class GetDailyMenuMealRequestHandler(IReadRepository<DailyMenuMeal> repository, IStringLocalizer<GetDailyMenuMealRequestHandler> localizer) : IRequestHandler<GetDailyMenuMealRequest, Result<DailyMenuMealDetailsDto>>
{
    private readonly IReadRepository<DailyMenuMeal> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<DailyMenuMealDetailsDto>> Handle(GetDailyMenuMealRequest request, CancellationToken cancellationToken)
    {
        TypeAdapterConfig config = new TypeAdapterConfig();
        config.NewConfig<DailyMenuMeal, DailyMenuMealDetailsDto>();
        //.Map(destination => destination.MealTypes, src => src.DailyMenuMealMealTypes.Select(x => x.MealType));

        var entity = await _repository.FirstOrDefaultAsync(new DailyMenuMealByIdSpec<DailyMenuMealDetailsDto>(request.Id), config, cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["DailyMenuMeal not found", request.Id]);

        return await Result<DailyMenuMealDetailsDto>.SuccessAsync(entity);
    }

}
