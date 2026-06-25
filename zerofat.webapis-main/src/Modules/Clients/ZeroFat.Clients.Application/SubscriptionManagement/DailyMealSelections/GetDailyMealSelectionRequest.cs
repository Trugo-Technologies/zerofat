using Ardalis.Specification;
using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.DailyMealSelections;

public class DailyMealSelectionByIdSpec<T> : Specification<DailyMealSelection, T>
{
    public DailyMealSelectionByIdSpec(DefaultIdType id)
    {
        Query.Where(p => p.Id == id);
    }
}

public class GetDailyMealSelectionRequest(DefaultIdType id) : IQuery<Result<DailyMealSelectionDetailsDto>>
{
    public DefaultIdType Id { get; set; } = id;
}

public class GetDailyMealSelectionRequestHandler(
    IRepositoryWithEvents<DailyMealSelection> repository, 
    IReadRepository<CustomMeal> customMealRepo, 
    IStringLocalizer<GetDailyMealSelectionRequestHandler> localizer) : IRequestHandler<GetDailyMealSelectionRequest, Result<DailyMealSelectionDetailsDto>>
{
    private readonly IRepositoryWithEvents<DailyMealSelection> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<DailyMealSelectionDetailsDto>> Handle(GetDailyMealSelectionRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new DailyMealSelectionByIdSpec<DailyMealSelectionDetailsDto>(request.Id), cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["DailyMealSelection not found", request.Id]);

        if(entity.CustomMealId.HasValue)
            entity.CustomMeal = await customMealRepo.FirstOrDefaultAsync(new ExpressionSpecificationProjecting<CustomMeal, CustomMealDto>(x => x.Id == entity.CustomMealId.Value), cancellationToken);

        return await Result<DailyMealSelectionDetailsDto>.SuccessAsync(entity);
    }

}
