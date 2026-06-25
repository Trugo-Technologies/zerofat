using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;

namespace ZeroFat.GymUp.Application.Creator.WorkoutBodyParts;

public class DeleteWorkoutBodyPartRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType Id { get; set; }
    public DeleteWorkoutBodyPartRequest(DefaultIdType id)
    {
        Id = id;
    }
}


public class DeleteWorkoutBodyPartRequestHandler(IRepositoryWithEvents<WorkoutBodyPart> repository, IStringLocalizer<DeleteWorkoutBodyPartRequestHandler> localizer) : IRequestHandler<DeleteWorkoutBodyPartRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<WorkoutBodyPart> _repository = repository;
    private readonly IStringLocalizer<DeleteWorkoutBodyPartRequestHandler> _localizer = localizer;

    public async Task<Result<DefaultIdType>> Handle(DeleteWorkoutBodyPartRequest request, CancellationToken cancellationToken)
    {
        var workoutBodyPart = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = workoutBodyPart ?? throw new NotFoundException(_localizer["Workout body part not found"]);

        await _repository.DeleteAsync(workoutBodyPart, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(workoutBodyPart.Id);
    }

}
