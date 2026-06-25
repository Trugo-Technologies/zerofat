using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Application.MeasurementUnits;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.Settings.MeasurementUnits;
public class GetMeasurementUnitRequest(DefaultIdType id) : IQuery<Result<MeasurementUnitDetailsDto>>
{
    public DefaultIdType Id { get; set; } = id;
}

public class GetMeasurementUnitRequestHandler(IRepositoryWithEvents<MeasurementUnit> repository, IStringLocalizer<GetMeasurementUnitRequestHandler> localizer) : IRequestHandler<GetMeasurementUnitRequest, Result<MeasurementUnitDetailsDto>>
{
    private readonly IRepositoryWithEvents<MeasurementUnit> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<MeasurementUnitDetailsDto>> Handle(GetMeasurementUnitRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new MeasurementUnitByIdSpec<MeasurementUnitDetailsDto>(request.Id), cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["MeasurementUnit not found", request.Id]);

        return await Result<MeasurementUnitDetailsDto>.SuccessAsync(entity);
    }
}
