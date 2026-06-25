using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using Microsoft.Extensions.Localization;

namespace ZeroFat.GymUp.Application.Creator.Workouts;
public class ActiveWorkoutRequest : ICommand<Result>
{
    public DefaultIdType Id { get; set; }
    public ActiveWorkoutRequest(DefaultIdType id)
    {
        Id = id;
    }
}

public class ActiveWorkoutRequestHandler(IRepository<Workout> repository, IStringLocalizer<ActiveWorkoutRequestHandler> localizer) : ICommandHandler<ActiveWorkoutRequest, Result>
{
    private readonly IRepository<Workout> _repository = repository;
    private readonly IStringLocalizer<ActiveWorkoutRequestHandler> _localizer = localizer;

    public async Task<Result> Handle(ActiveWorkoutRequest request, CancellationToken cancellationToken)
    {
        Workout? workout = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = workout ?? throw new NotFoundException(_localizer["Workout not found"]);


        workout.IsActive = !workout.IsActive;

        await _repository.UpdateAsync(workout, cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
