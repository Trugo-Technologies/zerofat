using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Catalog.Equipments;
public class DeleteEquipmentsRequest : ICommand<Result<bool>>
{
    public List<DefaultIdType> Ids { get; set; } = [];
}

public class DeleteEquipmentsRequestHandler(
    IRepositoryWithEvents<Equipment> repository,
    IStringLocalizer<DeleteEquipmentsRequestHandler> localizer,
    IReadRepository<WorkoutEquipment> workoutEquipmentRepo) : IRequestHandler<DeleteEquipmentsRequest, Result<bool>>
{

    public async Task<Result<bool>> Handle(DeleteEquipmentsRequest request, CancellationToken cancellationToken)
    {
        foreach (var ingId in request.Ids)
        {
            var equipment = await repository.GetByIdAsync(ingId, cancellationToken);
            if (equipment != null)
            {
                if (await workoutEquipmentRepo.AnyAsync(new ExpressionSpecification<WorkoutEquipment>(x => x.EquipmentId == equipment.Id), cancellationToken))
                    continue;

                await repository.DeleteAsync(equipment, withSaveChanges: false, cancellationToken: cancellationToken);
            }
        }

        await repository.SaveChangesAsync(cancellationToken);

        return await Result<bool>.SuccessAsync(data: true);
    }

}
