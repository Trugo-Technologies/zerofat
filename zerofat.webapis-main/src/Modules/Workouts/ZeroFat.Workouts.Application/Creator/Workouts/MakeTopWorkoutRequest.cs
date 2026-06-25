using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using Microsoft.Extensions.Localization;

namespace ZeroFat.GymUp.Application.Creator.Workouts;
public class MakeTopWorkoutRequest : ICommand<Result>
{
    public DefaultIdType Id { get; set; }
    public MakeTopWorkoutRequest(DefaultIdType id)
    {
        Id = id;
    }
}

public class MakeTopWorkoutRequestHandler(IRepository<Workout> repository, IStringLocalizer<MakeTopWorkoutRequestHandler> localizer) : ICommandHandler<MakeTopWorkoutRequest, Result>
{
    private readonly IRepository<Workout> _repository = repository;
    private readonly IStringLocalizer<MakeTopWorkoutRequestHandler> _localizer = localizer;

    public async Task<Result> Handle(MakeTopWorkoutRequest request, CancellationToken cancellationToken)
    {
        var workout = await _repository.GetByIdAsync(request.Id, cancellationToken);
        _ = workout ?? throw new NotFoundException(_localizer["Workout not found"]);

        workout.IsTop = !workout.IsTop;

        await _repository.UpdateAsync(workout, cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
