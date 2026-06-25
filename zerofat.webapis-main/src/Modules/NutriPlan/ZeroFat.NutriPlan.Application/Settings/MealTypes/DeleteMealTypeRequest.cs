using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.Settings.MealTypes;

public class DeleteMealTypeRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType Id { get; set; }
    public DeleteMealTypeRequest(DefaultIdType id) => Id = id;
}


public class DeleteMealTypeRequestHandler(IRepository<MealType> repository, IStringLocalizer<DeleteMealTypeRequestHandler> localizer) : IRequestHandler<DeleteMealTypeRequest, Result<DefaultIdType>>
{
    private readonly IRepository<MealType> _repository = repository;
    private readonly IStringLocalizer<DeleteMealTypeRequestHandler> _localizer = localizer;

    public async Task<Result<DefaultIdType>> Handle(DeleteMealTypeRequest request, CancellationToken cancellationToken)
    {
        var mealType = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = mealType ?? throw new NotFoundException(_localizer["MealType not found"]);

        await _repository.DeleteAsync(mealType, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(mealType.Id);
    }

}
