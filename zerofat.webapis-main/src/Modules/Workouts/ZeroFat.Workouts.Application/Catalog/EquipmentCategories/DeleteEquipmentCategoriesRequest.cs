using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Catalog.EquipmentCategories;
public class DeleteEquipmentCategoriesRequest : ICommand<Result<bool>>
{
    public List<DefaultIdType> Ids { get; set; } = [];
}

public class DeleteEquipmentCategoriesRequestHandler(
    IRepositoryWithEvents<EquipmentCategory> repository,
    IStringLocalizer<DeleteEquipmentCategoriesRequestHandler> localizer,
    IReadRepository<Equipment> equipmentRepo) : IRequestHandler<DeleteEquipmentCategoriesRequest, Result<bool>>
{

    public async Task<Result<bool>> Handle(DeleteEquipmentCategoriesRequest request, CancellationToken cancellationToken)
    {
        foreach (var ingId in request.Ids)
        {
            var equipmentCategory = await repository.GetByIdAsync(ingId, cancellationToken);
            if (equipmentCategory != null)
            {
                if (await equipmentRepo.AnyAsync(new ExpressionSpecification<Equipment>(x => x.CategoryId == equipmentCategory.Id), cancellationToken))
                    continue;

                await repository.DeleteAsync(equipmentCategory, withSaveChanges: false, cancellationToken: cancellationToken);
            }
        }

        await repository.SaveChangesAsync(cancellationToken);

        return await Result<bool>.SuccessAsync(data: true);
    }

}
