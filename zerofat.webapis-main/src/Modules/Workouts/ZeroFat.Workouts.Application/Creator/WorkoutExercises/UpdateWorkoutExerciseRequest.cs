using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Domain.Enums;

namespace ZeroFat.GymUp.Application.Creator.WorkoutExercises;

public class UpdateWorkoutExerciseRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType Id { get; set; }
    public int Index { get; set; }
    public int? Reps { get; set; }
    public int? Sets { get; set; }
    public int? Weight { get; set; }
    public int? DurationInSec { get; set; }
    public string? SetNameEn { get; set; }
    public string? SetNameAr { get; set; }

}


public class UpdateWorkoutExerciseRequestHandler(IRepositoryWithEvents<WorkoutExercise> repository, IStringLocalizer<UpdateWorkoutExerciseRequestHandler> localizer, IReadRepository<Exercise> exerciseRepo) : IRequestHandler<UpdateWorkoutExerciseRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<WorkoutExercise> _repository = repository;
    private readonly IReadRepository<Exercise> _exerciseRepo = exerciseRepo;
    private readonly IStringLocalizer<UpdateWorkoutExerciseRequestHandler> _localizer = localizer;

    public async Task<Result<DefaultIdType>> Handle(UpdateWorkoutExerciseRequest request, CancellationToken cancellationToken)
    {
        var workoutExer = await _repository.GetByIdAsync(request.Id, cancellationToken);
        _ = workoutExer ?? throw new NotFoundException(_localizer["Workout exercise not found"]);

        var exercise = await _exerciseRepo.GetByIdAsync(workoutExer.ExerciseId, cancellationToken);
        if (exercise is null)
            throw new NotFoundException(_localizer["Exercise not found"]);
        if (exercise.Type == ExerciseType.Duration && (!request.DurationInSec.HasValue || request.DurationInSec == 0))
            throw new BadRequestException(_localizer["Duration must have a value and bigger than 0"]);
        else if (exercise.Type == ExerciseType.Reps && (!request.Reps.HasValue || request.Reps == 0))
            throw new BadRequestException(_localizer["Reps must have a value and bigger than 0"]);
        else if (exercise.Type == ExerciseType.WeightAndReps && (!request.Weight.HasValue || request.Weight == 0 || !request.Reps.HasValue || request.Reps == 0))
            throw new BadRequestException(_localizer["Weight and reps must have a value and bigger than 0"]);

        if (request.Index != workoutExer.Index)
        {
            var set = await _repository.ListAsync(new ExpressionSpecification<WorkoutExercise>(x => x.WorkoutId == workoutExer.WorkoutId && x.Index >= request.Index), cancellationToken);
            foreach (var item in set)
                item.Index++;
        }

        workoutExer.Sets = request.Sets;
        workoutExer.Index = request.Index;
        workoutExer.Reps = request.Reps;
        workoutExer.Weight = request.Weight;
        workoutExer.DurationInSec = request.DurationInSec;
        workoutExer.SetNameEn = request.SetNameEn;
        workoutExer.SetNameAr = request.SetNameAr;

        await _repository.SaveChangesAsync(cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(workoutExer.Id);
    }
}

