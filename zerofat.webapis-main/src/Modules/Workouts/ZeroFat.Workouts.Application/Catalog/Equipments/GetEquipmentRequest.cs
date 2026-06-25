using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;

namespace ZeroFat.GymUp.Application.Catalog.Equipments;
public class GetEquipmentRequest(DefaultIdType id) : IQuery<Result<EquipmentDetailsDto>>
{
    public DefaultIdType Id { get; set; } = id;
}

public class GetEquipmentRequestHandler(IRepositoryWithEvents<Equipment> repository, IStringLocalizer<GetEquipmentRequestHandler> localizer) : IRequestHandler<GetEquipmentRequest, Result<EquipmentDetailsDto>>
{
    private readonly IRepositoryWithEvents<Equipment> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<EquipmentDetailsDto>> Handle(GetEquipmentRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new EquipmentByIdSpec<EquipmentDetailsDto>(request.Id), cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["Equipment not found", request.Id]);

        return await Result<EquipmentDetailsDto>.SuccessAsync(entity);
    }

}
