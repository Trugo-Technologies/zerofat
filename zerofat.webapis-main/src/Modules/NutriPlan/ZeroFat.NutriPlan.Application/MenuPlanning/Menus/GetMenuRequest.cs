using Mapster;
using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.MealPlanning;
using ZeroFat.NutriPlan.Domain.MenuPlanning;

namespace ZeroFat.NutriPlan.Application.MenuPlanning.Menus;
public class GetMenuRequest(Guid id) : IQuery<Result<MenuDetailsDto>>
{
    public Guid Id { get; set; } = id;
}

public class GetMenuRequestHandler(IReadRepository<Menu> repository, IStringLocalizer<GetMenuRequestHandler> localizer) : IRequestHandler<GetMenuRequest, Result<MenuDetailsDto>>
{
    private readonly IReadRepository<Menu> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<MenuDetailsDto>> Handle(GetMenuRequest request, CancellationToken cancellationToken)
    {
        TypeAdapterConfig config = new TypeAdapterConfig();
        config.NewConfig<Menu, MenuDetailsDto>();
                //.Map(destination => destination.MealTypes, src => src.MenuMealTypes.Select(x => x.MealType));

        var entity = await _repository.FirstOrDefaultAsync(new MenuByIdSpec<MenuDetailsDto>(request.Id), config, cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["Menu not found", request.Id]);

        return await Result<MenuDetailsDto>.SuccessAsync(entity);
    }

}
