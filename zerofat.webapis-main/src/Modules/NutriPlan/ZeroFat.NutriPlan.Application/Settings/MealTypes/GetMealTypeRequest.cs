using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.Settings.MealTypes;
public class GetMealTypeRequest(DefaultIdType id) : IQuery<Result<MealTypeDetailsDto>>
{
    public DefaultIdType Id { get; set; } = id;
}

public class GetMealTypeRequestHandler(IRepositoryWithEvents<MealType> repository, IStringLocalizer<GetMealTypeRequestHandler> localizer) : IRequestHandler<GetMealTypeRequest, Result<MealTypeDetailsDto>>
{
    private readonly IRepositoryWithEvents<MealType> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<MealTypeDetailsDto>> Handle(GetMealTypeRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new MealTypeByIdSpec<MealTypeDetailsDto>(request.Id), cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["MealType not found", request.Id]);

        return await Result<MealTypeDetailsDto>.SuccessAsync(entity);
    }
}
