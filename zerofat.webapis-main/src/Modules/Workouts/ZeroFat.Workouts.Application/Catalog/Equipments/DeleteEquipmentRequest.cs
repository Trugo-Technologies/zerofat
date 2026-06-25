using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Catalog.Equipments;

public class DeleteEquipmentRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType Id { get; set; }
    public DeleteEquipmentRequest(DefaultIdType id)
    {
        Id = id;
    }
}


public class DeleteEquipmentRequestHandler(IRepository<Equipment> repository, IStringLocalizer<DeleteEquipmentRequestHandler> localizer, IReadRepository<WorkoutEquipment> workoutEquipmentRepo) : IRequestHandler<DeleteEquipmentRequest, Result<DefaultIdType>>
{
    private readonly IRepository<Equipment> _repository = repository;
    private readonly IReadRepository<WorkoutEquipment> _workoutEquipmentRepo = workoutEquipmentRepo;
    private readonly IStringLocalizer<DeleteEquipmentRequestHandler> _localizer = localizer;

    public async Task<Result<DefaultIdType>> Handle(DeleteEquipmentRequest request, CancellationToken cancellationToken)
    {
        var eq = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = eq ?? throw new NotFoundException(_localizer["Equipment not found"]);

        bool used = await _workoutEquipmentRepo.AnyAsync(new ExpressionSpecification<WorkoutEquipment>(x => x.EquipmentId == eq.Id), cancellationToken);
        if (used)
            throw new BadRequestException(_localizer["Can not be deleted, because it's linked with workouts"]);

        await _repository.DeleteAsync(eq, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(eq.Id);
    }

}
