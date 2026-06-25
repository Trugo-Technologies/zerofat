using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.MealCustomizationGroups;
public class GetMealCustomizationGroupRequest(DefaultIdType id) : IQuery<Result<MealCustomizationGroupDetailsDto>>
{
    public DefaultIdType Id { get; set; } = id;
}

public class GetMealCustomizationGroupRequestHandler(IRepositoryWithEvents<MealCustomizationGroup> repository, IStringLocalizer<GetMealCustomizationGroupRequestHandler> localizer) : IRequestHandler<GetMealCustomizationGroupRequest, Result<MealCustomizationGroupDetailsDto>>
{
    private readonly IRepositoryWithEvents<MealCustomizationGroup> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<MealCustomizationGroupDetailsDto>> Handle(GetMealCustomizationGroupRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new MealCustomizationGroupByIdSpec<MealCustomizationGroupDetailsDto>(request.Id), cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["MealCustomizationGroup not found", request.Id]);

        return await Result<MealCustomizationGroupDetailsDto>.SuccessAsync(entity);
    }
}
