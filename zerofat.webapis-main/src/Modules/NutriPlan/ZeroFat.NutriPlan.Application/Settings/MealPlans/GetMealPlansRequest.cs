using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.Settings.MealPlans;

public class GetMealPlansRequest : IQuery<Result<List<MealPlanDto>>>
{
    public bool? IsActive { get; set; }
}

public class GetMealPlansRequestHandler(IReadRepository<MealPlan> repository)
    : IQueryHandler<GetMealPlansRequest, Result<List<MealPlanDto>>>
{
    public async Task<Result<List<MealPlanDto>>> Handle(GetMealPlansRequest request, CancellationToken cancellationToken)
    {
        var items = await repository.ListAsync(
            new MealPlansByFilterSpec(request.IsActive),
            cancellationToken);

        return await Result<List<MealPlanDto>>.SuccessAsync(items);
    }
}
