using ZeroFat.Application.Common.Persistence;
using Microsoft.Extensions.Localization;

namespace ZeroFat.GymUp.Application.Catalog.Equipments;
public class ActiveEquipmentsRequest : ICommand<Result>
{
    public List<DefaultIdType> Ids { get; set; } = [];
    public bool IsActive { get; set; }
}

public class ActiveEquipmentsRequestHandler(IRepository<Equipment> repository, IStringLocalizer<ActiveEquipmentsRequestHandler> localizer) : ICommandHandler<ActiveEquipmentsRequest, Result>
{

    public async Task<Result> Handle(ActiveEquipmentsRequest request, CancellationToken cancellationToken)
    {
        foreach (var ingId in request.Ids)
        {
            var equipment = await repository.GetByIdAsync(ingId, cancellationToken);
            if (equipment != null)
            {
                equipment.IsActive = request.IsActive;
            }
        }

        await repository.SaveChangesAsync(cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
