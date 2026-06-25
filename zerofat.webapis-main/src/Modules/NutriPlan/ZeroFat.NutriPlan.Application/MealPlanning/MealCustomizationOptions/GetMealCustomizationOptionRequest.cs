using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.MealCustomizationOptions;
public class GetMealCustomizationOptionRequest(DefaultIdType id) : IQuery<Result<MealCustomizationOptionDetailsDto>>
{
    public DefaultIdType Id { get; set; } = id;
}

public class GetMealCustomizationOptionRequestHandler(IRepositoryWithEvents<MealCustomizationOption> repository, IStringLocalizer<GetMealCustomizationOptionRequestHandler> localizer) : IRequestHandler<GetMealCustomizationOptionRequest, Result<MealCustomizationOptionDetailsDto>>
{
    private readonly IRepositoryWithEvents<MealCustomizationOption> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<MealCustomizationOptionDetailsDto>> Handle(GetMealCustomizationOptionRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new MealCustomizationOptionByIdSpec<MealCustomizationOptionDetailsDto>(request.Id), cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["MealCustomizationOption not found", request.Id]);

        return await Result<MealCustomizationOptionDetailsDto>.SuccessAsync(entity);
    }
}
