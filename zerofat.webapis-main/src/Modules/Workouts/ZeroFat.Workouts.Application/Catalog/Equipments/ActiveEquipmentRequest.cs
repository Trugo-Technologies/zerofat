using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using Microsoft.Extensions.Localization;

namespace ZeroFat.GymUp.Application.Catalog.Equipments;
public class ActiveEquipmentRequest : ICommand<Result>
{
    public DefaultIdType Id { get; set; }
    public ActiveEquipmentRequest(DefaultIdType id)
    {
        Id = id;
    }
}

public class ActiveEquipmentRequestHandler(IRepository<Equipment> repository, IStringLocalizer<ActiveEquipmentRequestHandler> localizer) : ICommandHandler<ActiveEquipmentRequest, Result>
{
    private readonly IRepository<Equipment> _repository = repository;
    private readonly IStringLocalizer<ActiveEquipmentRequestHandler> _localizer = localizer;

    public async Task<Result> Handle(ActiveEquipmentRequest request, CancellationToken cancellationToken)
    {
        var eq = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = eq ?? throw new NotFoundException(_localizer["Equipment not found"]);


        eq.IsActive = !eq.IsActive;

        await _repository.UpdateAsync(eq, cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
