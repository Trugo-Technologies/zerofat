using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Creator.Workouts;
public class DeleteWorkoutsRequest : ICommand<Result<bool>>
{
    public List<DefaultIdType> Ids { get; set; } = [];
}

public class DeleteWorkoutsRequestHandler(
    IRepositoryWithEvents<Workout> repository,
    IStringLocalizer<DeleteWorkoutsRequestHandler> localizer,
    IRepositoryWithEvents<PlanSchedule> planScheduleRepo,
    IRepositoryWithEvents<WorkoutExercise> workoutExerciseRepo,
    IRepositoryWithEvents<WorkoutEquipment> workoutEquipmentRepo,
    IRepositoryWithEvents<WorkoutBodyPart> workoutBodyPartRepo) : IRequestHandler<DeleteWorkoutsRequest, Result<bool>>
{

    public async Task<Result<bool>> Handle(DeleteWorkoutsRequest request, CancellationToken cancellationToken)
    {
        foreach (var ingId in request.Ids)
        {
            var workout = await repository.GetByIdAsync(ingId, cancellationToken);
            if (workout != null)
            {
                bool used = await planScheduleRepo.AnyAsync(new ExpressionSpecification<PlanSchedule>(x => x.WorkoutId == workout.Id), cancellationToken);
                if (used)
                    continue;

                var workoutExercises = await workoutExerciseRepo.ListAsync(new ExpressionSpecification<WorkoutExercise>(x => x.WorkoutId == workout.Id), cancellationToken);
                if (workoutExercises.Count != 0)
                {
                    await workoutExerciseRepo.DeleteRangeAsync(workoutExercises, withSaveChanges: false, cancellationToken: cancellationToken);
                }

                var workoutEquipments = await workoutEquipmentRepo.ListAsync(new ExpressionSpecification<WorkoutEquipment>(x => x.WorkoutId == workout.Id), cancellationToken);
                if (workoutEquipments.Count != 0)
                {
                    await workoutEquipmentRepo.DeleteRangeAsync(workoutEquipments, withSaveChanges: false, cancellationToken: cancellationToken);
                }

                var workoutBodyParts = await workoutBodyPartRepo.ListAsync(new ExpressionSpecification<WorkoutBodyPart>(x => x.WorkoutId == workout.Id), cancellationToken);
                if (workoutBodyParts.Count != 0)
                {
                    await workoutBodyPartRepo.DeleteRangeAsync(workoutBodyParts, withSaveChanges: false, cancellationToken: cancellationToken);
                }

                await repository.DeleteAsync(workout, withSaveChanges: false, cancellationToken: cancellationToken);
            }
        }

        await repository.SaveChangesAsync(cancellationToken);

        return await Result<bool>.SuccessAsync(data: true);
    }

}
