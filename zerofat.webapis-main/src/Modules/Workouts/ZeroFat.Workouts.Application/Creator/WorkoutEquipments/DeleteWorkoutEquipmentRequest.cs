using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;

namespace ZeroFat.GymUp.Application.Creator.WorkoutEquipments;

public class DeleteWorkoutEquipmentRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType Id { get; set; }
    public DeleteWorkoutEquipmentRequest(DefaultIdType id)
    {
        Id = id;
    }
}


public class DeleteWorkoutEquipmentRequestHandler(IRepositoryWithEvents<WorkoutEquipment> repository, IStringLocalizer<DeleteWorkoutEquipmentRequestHandler> localizer) : IRequestHandler<DeleteWorkoutEquipmentRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<WorkoutEquipment> _repository = repository;
    private readonly IStringLocalizer<DeleteWorkoutEquipmentRequestHandler> _localizer = localizer;

    public async Task<Result<DefaultIdType>> Handle(DeleteWorkoutEquipmentRequest request, CancellationToken cancellationToken)
    {
        var workoutEq = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = workoutEq ?? throw new NotFoundException(_localizer["Workout equipment not found"]);

        await _repository.DeleteAsync(workoutEq, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(workoutEq.Id);
    }

}
