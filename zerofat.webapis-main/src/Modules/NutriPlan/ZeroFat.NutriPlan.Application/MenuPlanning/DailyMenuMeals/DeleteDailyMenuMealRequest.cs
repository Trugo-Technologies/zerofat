using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.MenuPlanning;

namespace ZeroFat.NutriPlan.Application.MenuPlanning.DailyMenuMeals;

public class DeleteDailyMenuMealRequest : ICommand<Result<Guid>>
{
    public Guid Id { get; set; }
    public DeleteDailyMenuMealRequest(Guid id)
    {
        Id = id;
    }
}


public class DeleteDailyMenuMealRequestHandler(IRepository<DailyMenuMeal> repository, IStringLocalizer<DeleteDailyMenuMealRequestHandler> localizer) : IRequestHandler<DeleteDailyMenuMealRequest, Result<Guid>>
{
    private readonly IRepository<DailyMenuMeal> _repository = repository;
    private readonly IStringLocalizer<DeleteDailyMenuMealRequestHandler> _localizer = localizer;

    public async Task<Result<Guid>> Handle(DeleteDailyMenuMealRequest request, CancellationToken cancellationToken)
    {
        DailyMenuMeal? part = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = part ?? throw new NotFoundException(_localizer["DailyMenuMeal not found"]);

        await _repository.DeleteAsync(part, cancellationToken);

        return await Result<Guid>.SuccessAsync(part.Id);
    }

}
