using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using Microsoft.Extensions.Localization;

namespace ZeroFat.GymUp.Application.Catalog.EquipmentCategories;
public class ActiveEquipmentCategoryRequest : ICommand<Result>
{
    public DefaultIdType Id { get; set; }
    public ActiveEquipmentCategoryRequest(DefaultIdType id)
    {
        Id = id;
    }
}

public class ActiveEquipmentCategoryRequestHandler(IRepository<EquipmentCategory> repository, IStringLocalizer<ActiveEquipmentCategoryRequestHandler> localizer) : ICommandHandler<ActiveEquipmentCategoryRequest, Result>
{
    private readonly IRepository<EquipmentCategory> _repository = repository;
    private readonly IStringLocalizer<ActiveEquipmentCategoryRequestHandler> _localizer = localizer;

    public async Task<Result> Handle(ActiveEquipmentCategoryRequest request, CancellationToken cancellationToken)
    {
        var cate = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = cate ?? throw new NotFoundException(_localizer["Category not found"]);


        cate.IsActive = !cate.IsActive;

        await _repository.UpdateAsync(cate, cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
