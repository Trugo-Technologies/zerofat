using Mapster;
using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.MenuPlanning;

namespace ZeroFat.NutriPlan.Application.MenuPlanning.DailyMenus;
public class GetDailyMenuRequest(Guid id) : IQuery<Result<DailyMenuDetailsDto>>
{
    public Guid Id { get; set; } = id;
}

public class GetDailyMenuRequestHandler(IReadRepository<DailyMenu> repository, IStringLocalizer<GetDailyMenuRequestHandler> localizer) : IRequestHandler<GetDailyMenuRequest, Result<DailyMenuDetailsDto>>
{
    private readonly IReadRepository<DailyMenu> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<DailyMenuDetailsDto>> Handle(GetDailyMenuRequest request, CancellationToken cancellationToken)
    {
        TypeAdapterConfig config = new TypeAdapterConfig();
        config.NewConfig<DailyMenu, DailyMenuDetailsDto>();
                        //.Map(destination => destination.MealTypes, src => src.DailyMenuMealTypes.Select(x => x.MealType));

        var entity = await _repository.FirstOrDefaultAsync(new DailyMenuByIdSpec<DailyMenuDetailsDto>(request.Id), config, cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["DailyMenu not found", request.Id]);

        return await Result<DailyMenuDetailsDto>.SuccessAsync(entity);
    }

}
