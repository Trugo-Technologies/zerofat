using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.Meals;

public class DeleteMealRequest : ICommand<Result<Guid>>
{
    public Guid Id { get; set; }
    public DeleteMealRequest(Guid id)
    {
        Id = id;
    }
}


public class DeleteMealRequestHandler(
    IRepository<Meal> repository,
    IRepositoryWithEvents<MealAllergen> mealAllergenRepo,
    IStringLocalizer<DeleteMealRequestHandler> localizer) : IRequestHandler<DeleteMealRequest, Result<Guid>>
{
    private readonly IRepository<Meal> _repository = repository;
    private readonly IRepositoryWithEvents<MealAllergen> _mealAllergenRepo = mealAllergenRepo;
    private readonly IStringLocalizer<DeleteMealRequestHandler> _localizer = localizer;

    public async Task<Result<Guid>> Handle(DeleteMealRequest request, CancellationToken cancellationToken)
    {
        Meal? meal = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = meal ?? throw new NotFoundException(_localizer["Meal not found"]);

        List<MealAllergen> mealAllergens = await _mealAllergenRepo.ListAsync(new ExpressionSpecification<MealAllergen>(x => x.MealId == request.Id), cancellationToken: cancellationToken);
        await _mealAllergenRepo.DeleteRangeAsync(mealAllergens, withSaveChanges: false, cancellationToken: cancellationToken);

        await _repository.DeleteAsync(meal, cancellationToken);

        return await Result<Guid>.SuccessAsync(meal.Id);
    }

}
