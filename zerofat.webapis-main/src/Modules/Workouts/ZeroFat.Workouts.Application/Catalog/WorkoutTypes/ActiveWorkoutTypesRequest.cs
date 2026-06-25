using ZeroFat.Application.Common.Persistence;
using Microsoft.Extensions.Localization;

namespace ZeroFat.GymUp.Application.Catalog.WorkoutTypes;
public class ActiveWorkoutTypesRequest : ICommand<Result>
{
    public List<DefaultIdType> Ids { get; set; } = [];
    public bool IsActive { get; set; }
}

public class ActiveWorkoutTypesRequestHandler(IRepository<WorkoutType> repository, IStringLocalizer<ActiveWorkoutTypesRequestHandler> localizer) : ICommandHandler<ActiveWorkoutTypesRequest, Result>
{

    public async Task<Result> Handle(ActiveWorkoutTypesRequest request, CancellationToken cancellationToken)
    {
        foreach (var ingId in request.Ids)
        {
            var workoutType = await repository.GetByIdAsync(ingId, cancellationToken);
            if (workoutType != null)
            {
                workoutType.IsActive = request.IsActive;
            }
        }

        await repository.SaveChangesAsync(cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
