using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.GymUp.Application.Creator.Workouts;
public class MakeWorkoutOfTheDayRequest : ICommand<Result>
{
    public DefaultIdType Id { get; set; }
    public MakeWorkoutOfTheDayRequest(DefaultIdType id)
    {
        Id = id;
    }
}

public class MakeWorkoutOfTheDayRequestHandler(IRepository<Workout> repository, IStringLocalizer<MakeWorkoutOfTheDayRequestHandler> localizer) : ICommandHandler<MakeWorkoutOfTheDayRequest, Result>
{
    private readonly IRepository<Workout> _repository = repository;
    private readonly IStringLocalizer<MakeWorkoutOfTheDayRequestHandler> _localizer = localizer;

    public async Task<Result> Handle(MakeWorkoutOfTheDayRequest request, CancellationToken cancellationToken)
    {
        var workout = await _repository.GetByIdAsync(request.Id, cancellationToken);
        _ = workout ?? throw new NotFoundException(_localizer["Workout not found"]);

        workout.IsWorkoutOfTheDay = !workout.IsWorkoutOfTheDay;

        if (workout.IsWorkoutOfTheDay)
        {
            var old = await _repository.FirstOrDefaultAsync(new ExpressionSpecification<Workout>(x => x.IsWorkoutOfTheDay && x.Id != workout.Id), cancellationToken);
            if (old != null)
            {
                old.IsWorkoutOfTheDay = false;
                await _repository.UpdateAsync(old, cancellationToken);
            }
        }

        await _repository.UpdateAsync(workout, cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
