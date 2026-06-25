using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.Settings.MealTypes;
public class ActiveMealTypeRequest : ICommand<Result>
{
    public DefaultIdType Id { get; set; }
    public ActiveMealTypeRequest(DefaultIdType id) => Id = id;
}

public class ActiveMealTypeRequestHandler(IRepository<MealType> repository, IStringLocalizer<ActiveMealTypeRequestHandler> localizer) : ICommandHandler<ActiveMealTypeRequest, Result>
{
    private readonly IRepository<MealType> _repository = repository;
    private readonly IStringLocalizer<ActiveMealTypeRequestHandler> _localizer = localizer;

    public async Task<Result> Handle(ActiveMealTypeRequest request, CancellationToken cancellationToken)
    {
        var mealType = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = mealType ?? throw new NotFoundException(_localizer["MealType not found"]);

        mealType.IsActive = !mealType.IsActive;

        await _repository.UpdateAsync(mealType, cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
