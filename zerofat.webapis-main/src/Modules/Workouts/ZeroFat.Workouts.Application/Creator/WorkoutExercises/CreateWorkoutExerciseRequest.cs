using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;

namespace ZeroFat.GymUp.Application.Creator.WorkoutExercises;
public class CreateWorkoutExerciseRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType WorkoutId { get; set; }
    public List<CreateWorkoutExerciseItem> Items { get; set; } = new();

}
public class CreateWorkoutExerciseItem
{
    public DefaultIdType ExerciseId { get; set; }
    public string? SetNameEn { get; set; }
    public string? SetNameAr { get; set; }
    public int? SetIndex { get; set; }
    public int? Reps { get; set; }
    public int? Sets { get; set; }
    public int? DurationInSec { get; set; }
    public int? Weight { get; set; }
}

public class CreateWorkoutExerciseRequestValidator : CustomValidator<CreateWorkoutExerciseRequest>
{
    public CreateWorkoutExerciseRequestValidator(IReadRepository<Workout> workoutRepo, IReadRepository<Exercise> exerciseRepo, IStringLocalizer<CreateWorkoutExerciseRequestValidator> localizer)
    {

        RuleFor(u => u.WorkoutId)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (id, _) => await workoutRepo.AnyAsync(new ExpressionSpecification<Workout>(x => x.Id == id), _))
                .WithMessage(localizer["Workout not found"]);

        RuleFor(u => u.Items)
           .NotNull()
           .WithMessage(localizer["Workout Exercises are required."])
           .Must(workoutExercises => workoutExercises.Count > 0)
           .WithMessage(localizer["At least one workout Exercise is required."]);

        RuleForEach(x => x.Items)
            .CustomAsync(async (item, context, cancellationToken) =>
            {
                // Check ExerciseId not empty
                if (item.ExerciseId == Guid.Empty)
                {
                    context.AddFailure(nameof(item.ExerciseId), localizer["Exercise Id is required."]);
                    return;
                }

                // Check Exercise exists
                var exercise = await exerciseRepo.FirstOrDefaultAsync(new ExpressionSpecification<Exercise>(x => x.Id == item.ExerciseId), cancellationToken);

                if (exercise == null)
                {
                    context.AddFailure(nameof(item.ExerciseId), localizer["Invalid Exercise ID."]);
                    return;
                }

                if (exercise.Type == ExerciseType.Duration && item.DurationInSec.GetValueOrDefault() == 0)
                {
                    context.AddFailure(localizer["Duration must have a value and be bigger than 0"]);
                }
                else if (exercise.Type == ExerciseType.Reps && item.Reps.GetValueOrDefault() == 0)
                {
                    context.AddFailure(localizer["Reps must have a value and be bigger than 0"]);
                }
                else if (exercise.Type == ExerciseType.WeightAndReps && (item.Weight.GetValueOrDefault() == 0 || item.Reps.GetValueOrDefault() == 0))
                {
                    context.AddFailure(localizer["Weight and reps must have a value and be bigger than 0"]);
                }

            });

        //RuleForEach(u => u.Items)
        //   .ChildRules(workoutExercise =>
        //   {
        //       workoutExercise.RuleFor(mt => mt.ExerciseId)
        //               .NotEmpty()
        //                   .WithMessage(localizer["Exercise Id is required."])
        //               .MustAsync(async (id, _) => await exerciseRepo.AnyAsync(new ExpressionSpecification<Exercise>(x => x.Id == id), _))
        //                    .WithMessage(localizer["Invalid Exercise ID."]);

        //       workoutExercise.CustomAsync(async (item, context, cancellationToken) =>
        //       {
        //           var exercise = await exerciseRepo.FirstOrDefaultAsync(
        //               new ExpressionSpecification<Exercise>(x => x.Id == item.ExerciseId), cancellationToken);

        //           if (exercise == null)
        //               return; // Already handled in previous validation

        //           if (exercise.Type == ExerciseType.Duration &&
        //               (!item.DurationInSec.HasValue || item.DurationInSec == 0))
        //           {
        //               context.AddFailure(localizer["Duration must have a value and be bigger than 0"]);
        //           }
        //           else if (exercise.Type == ExerciseType.Reps &&
        //               (!item.Reps.HasValue || item.Reps == 0))
        //           {
        //               context.AddFailure(localizer["Reps must have a value and be bigger than 0"]);
        //           }
        //           else if (exercise.Type == ExerciseType.WeightAndReps &&
        //               (item.Weight.GetValueOrDefault() == 0 || item.Reps.GetValueOrDefault() == 0))
        //           {
        //               context.AddFailure(localizer["Weight and reps must have a value and be bigger than 0"]);
        //           }
        //       });
        //   });
    }
}


public class CreateWorkoutExerciseRequestHandler(
    IRepositoryWithEvents<WorkoutExercise> repository,
    IReadRepository<Workout> workoutRepo,
    IStringLocalizer<CreateWorkoutExerciseRequestHandler> localizer) : IRequestHandler<CreateWorkoutExerciseRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<WorkoutExercise> _repository = repository;
    private readonly IReadRepository<Workout> _workoutRepo = workoutRepo;
    private readonly IStringLocalizer<CreateWorkoutExerciseRequestHandler> _localizer = localizer;

    public async Task<Result<DefaultIdType>> Handle(CreateWorkoutExerciseRequest request, CancellationToken cancellationToken)
    {
        var workout = await _workoutRepo.FirstOrDefaultAsync(new ExpressionSpecification<Workout>(x => x.Id == request.WorkoutId), cancellationToken) ?? default!;

        // Get existing workout exercises to determine next index
        var existingExercises = await _repository.ListAsync(
            new ExpressionSpecification<WorkoutExercise>(x => x.WorkoutId == request.WorkoutId),
            cancellationToken
        );
        int index = existingExercises.Count > 0 ? existingExercises.Max(x => x.Index) + 1 : 1;

        var workoutExerecises = new List<WorkoutExercise>();

        foreach (var item in request.Items)
        {
            var workoutExercise = new WorkoutExercise
            {
                SetIndex = item.SetIndex ?? item.Sets, // Default SetIndex to Sets if null

                WorkoutId = request.WorkoutId,
                Sets = item.Sets,
                Reps = item.Reps,
                Weight = item.Weight,
                DurationInSec = item.DurationInSec,
                ExerciseId = item.ExerciseId,
                Index = index,
                SetNameAr = item.SetNameAr,
                SetNameEn = item.SetNameEn
            };

            if (workoutExercise.SetNameAr == null &&
                workoutExercise.SetIndex.HasValue &&
                workout.SetNamesAr != null &&
                workout.SetNamesEn != null)
            {
                int setIdx = workoutExercise.SetIndex.Value - 1;
                workoutExercise.SetNameAr = workout.SetNamesAr.ElementAtOrDefault(setIdx);
                workoutExercise.SetNameEn = workout.SetNamesEn.ElementAtOrDefault(setIdx);
            }

            workoutExerecises.Add(workoutExercise);

            index++;
        }

        await _repository.AddRangeAsync(workoutExerecises, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(request.WorkoutId);
    }
}
