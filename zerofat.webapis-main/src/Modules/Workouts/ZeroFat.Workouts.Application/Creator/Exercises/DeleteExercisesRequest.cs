using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Creator.Exercises;
public class DeleteExercisesRequest : ICommand<Result<bool>>
{
    public List<DefaultIdType> Ids { get; set; } = [];
}

public class DeleteExercisesRequestHandler(
    IRepositoryWithEvents<Exercise> repository,
    IStringLocalizer<DeleteExercisesRequestHandler> localizer,
    IReadRepository<WorkoutExercise> workoutExerciseRepo,
    IRepositoryWithEvents<ExerciseBodyPart> exerciseBodyPartRepo) : IRequestHandler<DeleteExercisesRequest, Result<bool>>
{

    public async Task<Result<bool>> Handle(DeleteExercisesRequest request, CancellationToken cancellationToken)
    {
        foreach (var ingId in request.Ids)
        {
            var exercise = await repository.GetByIdAsync(ingId, cancellationToken);
            if (exercise != null)
            {
                if (await workoutExerciseRepo.AnyAsync(new ExpressionSpecification<WorkoutExercise>(x => x.ExerciseId == exercise.Id), cancellationToken))
                    continue;

                var bodyParts = await exerciseBodyPartRepo.ListAsync(new ExpressionSpecification<ExerciseBodyPart>(x => x.ExerciseId == exercise.Id), cancellationToken);
                if (bodyParts.Count != 0)
                    await exerciseBodyPartRepo.DeleteRangeAsync(bodyParts, withSaveChanges: false, cancellationToken: cancellationToken);

                await repository.DeleteAsync(exercise, withSaveChanges: false, cancellationToken: cancellationToken);
            }
        }

        await repository.SaveChangesAsync(cancellationToken);

        return await Result<bool>.SuccessAsync(data: true);
    }

}
