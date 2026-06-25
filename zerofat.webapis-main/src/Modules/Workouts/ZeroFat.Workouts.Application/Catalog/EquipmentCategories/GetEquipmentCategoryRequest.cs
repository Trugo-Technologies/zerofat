using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;

namespace ZeroFat.GymUp.Application.Catalog.EquipmentCategories;

public class GetEquipmentCategoryRequest(DefaultIdType id) : IQuery<Result<EquipmentCategoryDetailsDto>>
{
    public DefaultIdType Id { get; set; } = id;
}

public class GetEquipmentCategoryRequestHandler(IReadRepository<EquipmentCategory> repository, IStringLocalizer<GetEquipmentCategoryRequestHandler> localizer) : IRequestHandler<GetEquipmentCategoryRequest, Result<EquipmentCategoryDetailsDto>>
{
    private readonly IReadRepository<EquipmentCategory> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<EquipmentCategoryDetailsDto>> Handle(GetEquipmentCategoryRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new EquipmentCategoryByIdSpec<EquipmentCategoryDetailsDto>(request.Id), cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["EquipmentCategory not found", request.Id]);

        return await Result<EquipmentCategoryDetailsDto>.SuccessAsync(entity);
    }

}
