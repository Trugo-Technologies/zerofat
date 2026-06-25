using ZeroFat.Application.Common.Persistence;
using Microsoft.Extensions.Localization;

namespace ZeroFat.GymUp.Application.Creator.Workouts;
public class ActiveWorkoutsRequest : ICommand<Result>
{
    public List<DefaultIdType> Ids { get; set; } = [];
    public bool IsActive { get; set; }
}

public class ActiveWorkoutsRequestHandler(
    IRepository<Workout> repository,
    IStringLocalizer<ActiveWorkoutsRequestHandler> localizer) : ICommandHandler<ActiveWorkoutsRequest, Result>
{

    public async Task<Result> Handle(ActiveWorkoutsRequest request, CancellationToken cancellationToken)
    {
        foreach (var ingId in request.Ids)
        {
            var workout = await repository.GetByIdAsync(ingId, cancellationToken);
            if (workout != null)
            {
                workout.IsActive = request.IsActive;
            }
        }

        await repository.SaveChangesAsync(cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
