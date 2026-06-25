using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.MeasurementUnits;

public class DeleteMeasurementUnitRequest : ICommand<Result<Guid>>
{
    public Guid Id { get; set; }
    public DeleteMeasurementUnitRequest(Guid id) => Id = id;
}


public class DeleteMeasurementUnitRequestHandler(
    IRepository<MeasurementUnit> repository, 
    IStringLocalizer<DeleteMeasurementUnitRequestHandler> localizer) : IRequestHandler<DeleteMeasurementUnitRequest, Result<Guid>>
{
    private readonly IRepository<MeasurementUnit> _repository = repository;
    private readonly IStringLocalizer<DeleteMeasurementUnitRequestHandler> _localizer = localizer;

    public async Task<Result<Guid>> Handle(DeleteMeasurementUnitRequest request, CancellationToken cancellationToken)
    {
        var part = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = part ?? throw new NotFoundException(_localizer["MeasurementUnit not found"]);

        if(part.IsDefault)
            throw new BadRequestException(_localizer["Default MeasurementUnit can't be deleted"]);
        
        await _repository.DeleteAsync(part, cancellationToken);

        return await Result<Guid>.SuccessAsync(part.Id);
    }

}
