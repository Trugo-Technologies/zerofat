using ZeroFat.Application.Common.Persistence;
using Microsoft.Extensions.Localization;

namespace ZeroFat.GymUp.Application.Catalog.EquipmentCategories;
public class ActiveEquipmentCategoriesRequest : ICommand<Result>
{
    public List<DefaultIdType> Ids { get; set; } = [];
    public bool IsActive { get; set; }
}

public class ActiveEquipmentCategoriesRequestHandler(IRepository<EquipmentCategory> repository, IStringLocalizer<ActiveEquipmentCategoriesRequestHandler> localizer) : ICommandHandler<ActiveEquipmentCategoriesRequest, Result>
{

    public async Task<Result> Handle(ActiveEquipmentCategoriesRequest request, CancellationToken cancellationToken)
    {
        foreach (var ingId in request.Ids)
        {
            var equipmentCategory = await repository.GetByIdAsync(ingId, cancellationToken);
            if (equipmentCategory != null)
            {
                equipmentCategory.IsActive = request.IsActive;
            }
        }

        await repository.SaveChangesAsync(cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
