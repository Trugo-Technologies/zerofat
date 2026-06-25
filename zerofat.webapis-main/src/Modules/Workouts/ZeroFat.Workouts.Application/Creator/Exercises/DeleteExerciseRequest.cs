using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Creator.Exercises;

public class DeleteExerciseRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType Id { get; set; }
    public DeleteExerciseRequest(DefaultIdType id)
    {
        Id = id;
    }
}


public class DeleteExerciseRequestHandler(IRepository<Exercise> repository, IStringLocalizer<DeleteExerciseRequestHandler> localizer, IReadRepository<WorkoutExercise> workoutExerRepo, IRepository<ExerciseBodyPart> exerciseBodyRepo) : IRequestHandler<DeleteExerciseRequest, Result<DefaultIdType>>
{
    private readonly IRepository<Exercise> _repository = repository;
    private readonly IReadRepository<WorkoutExercise> _workoutExerRepo = workoutExerRepo;
    private readonly IRepository<ExerciseBodyPart> _exerciseBodyRepo = exerciseBodyRepo;

    private readonly IStringLocalizer<DeleteExerciseRequestHandler> _localizer = localizer;

    public async Task<Result<DefaultIdType>> Handle(DeleteExerciseRequest request, CancellationToken cancellationToken)
    {
        var exercise = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = exercise ?? throw new NotFoundException(_localizer["Exercise not found"]);

        bool used = await _workoutExerRepo.AnyAsync(new ExpressionSpecification<WorkoutExercise>(x => x.ExerciseId == exercise.Id), cancellationToken);
        if (used)
            throw new BadRequestException(_localizer["Can not be deleted, because the exercise is linked with workouts"]);

        var bodyParts = await _exerciseBodyRepo.ListAsync(new ExpressionSpecification<ExerciseBodyPart>(x => x.ExerciseId == exercise.Id), cancellationToken);
        if (bodyParts.Count != 0)
            await _exerciseBodyRepo.DeleteRangeAsync(bodyParts, cancellationToken);


        await _repository.DeleteAsync(exercise, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(exercise.Id);
    }

}
