using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Creator.WorkoutExercises;

public class DeleteWorkoutExerciseRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType Id { get; set; }
    public DeleteWorkoutExerciseRequest(DefaultIdType id)
    {
        Id = id;
    }
}


public class DeleteWorkoutExerciseRequestHandler(IRepositoryWithEvents<WorkoutExercise> repository, IStringLocalizer<DeleteWorkoutExerciseRequestHandler> localizer) : IRequestHandler<DeleteWorkoutExerciseRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<WorkoutExercise> _repository = repository;
    private readonly IStringLocalizer<DeleteWorkoutExerciseRequestHandler> _localizer = localizer;

    public async Task<Result<DefaultIdType>> Handle(DeleteWorkoutExerciseRequest request, CancellationToken cancellationToken)
    {
        var workoutExer = await _repository.GetByIdAsync(request.Id, cancellationToken);
        _ = workoutExer ?? throw new NotFoundException(_localizer["Workout exercise not found"]);

        // After deleting the workout exercise
        var remainingExercises = await _repository.ListAsync(
            new ExpressionSpecification<WorkoutExercise>(x => x.WorkoutId == workoutExer.WorkoutId && x.Id != request.Id),
            cancellationToken
        );

        // Reorder indexes sequentially
        int newIndex = 1;
        foreach (var exercise in remainingExercises.OrderBy(x => x.Index))
        {
            exercise.Index = newIndex++;
        }

        await _repository.DeleteAsync(workoutExer, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(workoutExer.Id);
    }

}
