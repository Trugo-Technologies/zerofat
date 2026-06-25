using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Extensions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.Settings.MealPlans;
public class PublishMealPlanRequest : ICommand<Result>
{
    public DefaultIdType Id { get; set; }
    public PublishMealPlanRequest(DefaultIdType id)
    {
        Id = id;
    }
}

public class PublishMealPlanRequestHandler(
    IRepository<MealPlan> repository,
    IStringLocalizer<PublishMealPlanRequestHandler> localizer,
    IStripeService stripeService) : ICommandHandler<PublishMealPlanRequest, Result>
{
    private readonly IRepository<MealPlan> _repository = repository;
    private readonly IStripeService _stripeService = stripeService;
    private readonly IStringLocalizer<PublishMealPlanRequestHandler> _localizer = localizer;

    public async Task<Result> Handle(PublishMealPlanRequest request, CancellationToken cancellationToken)
    {
        var mealPlan = await _repository.GetByIdAsync(request.Id, cancellationToken);
        _ = mealPlan ?? throw new NotFoundException(_localizer["Invalid MealPlan ID"]);
        if (mealPlan.StripeId.HasValue())
        {
            throw new BadRequestException(_localizer["plan already published."]);
        }

        mealPlan.StripeId = await _stripeService.CreateProductOnStripe(mealPlan.Code, mealPlan.NameEn, mealPlan.Id.ToString());

        await _repository.UpdateAsync(mealPlan, cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
