using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.Settings.MealPlans;

public class DeleteMealPlanRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType Id { get; set; }
    public DeleteMealPlanRequest(DefaultIdType id) => Id = id;
}


public class DeleteMealPlanRequestHandler(IRepository<MealPlan> repository, IStringLocalizer<DeleteMealPlanRequestHandler> localizer) : IRequestHandler<DeleteMealPlanRequest, Result<DefaultIdType>>
{
    private readonly IRepository<MealPlan> _repository = repository;
    private readonly IStringLocalizer<DeleteMealPlanRequestHandler> _localizer = localizer;

    public async Task<Result<DefaultIdType>> Handle(DeleteMealPlanRequest request, CancellationToken cancellationToken)
    {
        var MealPlan = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = MealPlan ?? throw new NotFoundException(_localizer["MealPlan not found"]);

        await _repository.DeleteAsync(MealPlan, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(MealPlan.Id);
    }

}
