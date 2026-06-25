using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Creator.Workouts;
public class DeleteWorkoutRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType Id { get; set; }
    public DeleteWorkoutRequest(DefaultIdType id) => Id = id;
}


public class DeleteWorkoutRequestHandler(IRepository<Workout> repository, IStringLocalizer<DeleteWorkoutRequestHandler> localizer, IReadRepository<PlanSchedule> planScRepo, IReadRepository<WorkoutExercise> workoutExRepo, IRepository<WorkoutEquipment> workoutEqRepo, IRepository<WorkoutBodyPart> workoutBodyRepo) : IRequestHandler<DeleteWorkoutRequest, Result<DefaultIdType>>
{
    private readonly IRepository<Workout> _repository = repository;
    private readonly IRepository<WorkoutEquipment> _workoutEqRepo = workoutEqRepo;
    private readonly IRepository<WorkoutBodyPart> _workoutBodyRepo = workoutBodyRepo;
    private readonly IReadRepository<PlanSchedule> _planScRepo = planScRepo;
    private readonly IReadRepository<WorkoutExercise> _workoutExRepo = workoutExRepo;
    private readonly IStringLocalizer<DeleteWorkoutRequestHandler> _localizer = localizer;

    public async Task<Result<DefaultIdType>> Handle(DeleteWorkoutRequest request, CancellationToken cancellationToken)
    {
        var workout = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = workout ?? throw new NotFoundException(_localizer["Workout not found"]);

        bool used = await _planScRepo.AnyAsync(new ExpressionSpecification<PlanSchedule>(x => x.WorkoutId == workout.Id), cancellationToken);
        if (used)
            throw new BadRequestException(_localizer["Can not be deleted, because the workout is in plans"]);

        used = await _workoutExRepo.AnyAsync(new ExpressionSpecification<WorkoutExercise>(x => x.WorkoutId == workout.Id), cancellationToken);
        if (used)
            throw new BadRequestException(_localizer["Can not be deleted, because the workout has exercises"]);

        var equipments = await _workoutEqRepo.ListAsync(new ExpressionSpecification<WorkoutEquipment>(x => x.WorkoutId == workout.Id), cancellationToken);
        if (equipments.Count != 0)
            await _workoutEqRepo.DeleteRangeAsync(equipments, cancellationToken);

        var bodyParts = await _workoutBodyRepo.ListAsync(new ExpressionSpecification<WorkoutBodyPart>(x => x.WorkoutId == workout.Id), cancellationToken);
        if (bodyParts.Count != 0)
            await _workoutBodyRepo.DeleteRangeAsync(bodyParts, cancellationToken);

        await _repository.DeleteAsync(workout, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(workout.Id);
    }

}
