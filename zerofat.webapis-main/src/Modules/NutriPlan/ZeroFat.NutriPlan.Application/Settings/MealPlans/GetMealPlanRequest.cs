using Mapster;
using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.Settings.MealPlans;
public class GetMealPlanRequest(DefaultIdType id) : IQuery<Result<MealPlanDetailsDto>>
{
    public DefaultIdType Id { get; set; } = id;
}

public class GetMealPlanRequestHandler(IRepositoryWithEvents<MealPlan> repository, IStringLocalizer<GetMealPlanRequestHandler> localizer) : IRequestHandler<GetMealPlanRequest, Result<MealPlanDetailsDto>>
{
    private readonly IRepositoryWithEvents<MealPlan> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<MealPlanDetailsDto>> Handle(GetMealPlanRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new MealPlanByIdSpec<MealPlanDetailsDto>(request.Id), cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["MealPlan not found", request.Id]);

        return await Result<MealPlanDetailsDto>.SuccessAsync(entity);
    }
}
