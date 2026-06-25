using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.NutriPlan.Domain.MenuPlanning;

namespace ZeroFat.NutriPlan.Application.MenuPlanning.DailyMenuMeals;
public class SetDailyMenuMealAsDefaulRequest : ICommand<Result<Guid>>
{
    public Guid Id { get; set; }

}

public class SetDailyMenuMealAsDefaulRequestValidator : CustomValidator<SetDailyMenuMealAsDefaulRequest>
{
    public SetDailyMenuMealAsDefaulRequestValidator(
        IStringLocalizer<SetDailyMenuMealAsDefaulRequestValidator> localaizer)
    {

    }
}

public class SetDailyMenuMealAsDefaulRequestHandler(
    IRepositoryWithEvents<DailyMenuMeal> repository,
    IStringLocalizer<SetDailyMenuMealAsDefaulRequestHandler> localizer) : IRequestHandler<SetDailyMenuMealAsDefaulRequest, Result<Guid>>
{
    private readonly IRepositoryWithEvents<DailyMenuMeal> _repository = repository;
    private readonly IStringLocalizer<SetDailyMenuMealAsDefaulRequestHandler> _localizer = localizer;

    public async Task<Result<Guid>> Handle(SetDailyMenuMealAsDefaulRequest request, CancellationToken cancellationToken)
    {
        DailyMenuMeal? entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["DailyMenuMeal not found"]);

        entity.IsDefault = !entity.IsDefault;

        if (entity.IsDefault)
        {
            DailyMenuMeal? old = await _repository.FirstOrDefaultAsync(new ExpressionSpecification<DailyMenuMeal>(x => x.IsDefault && x.Id != entity.Id && x.DailyMenuId == entity.DailyMenuId), cancellationToken);
            if (old != null)
            {
                old.IsDefault = false;
            }
        }

        await _repository.SaveChangesAsync( cancellationToken);

        return await Result<Guid>.SuccessAsync(entity.Id);
    }
}

