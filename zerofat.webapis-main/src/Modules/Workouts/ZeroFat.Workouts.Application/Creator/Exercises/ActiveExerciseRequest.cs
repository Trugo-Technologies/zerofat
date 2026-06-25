using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using Microsoft.Extensions.Localization;

namespace ZeroFat.GymUp.Application.Creator.Exercises;
public class ActiveExerciseRequest : ICommand<Result>
{
    public DefaultIdType Id { get; set; }
    public ActiveExerciseRequest(DefaultIdType id) => Id = id;
}

public class ActiveExerciseRequestHandler(IRepository<Exercise> repository, IStringLocalizer<ActiveExerciseRequestHandler> localizer) : ICommandHandler<ActiveExerciseRequest, Result>
{
    private readonly IRepository<Exercise> _repository = repository;
    private readonly IStringLocalizer<ActiveExerciseRequestHandler> _localizer = localizer;

    public async Task<Result> Handle(ActiveExerciseRequest request, CancellationToken cancellationToken)
    {
        var exercise = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = exercise ?? throw new NotFoundException(_localizer["Exercise not found"]);


        exercise.IsActive = !exercise.IsActive;

        await _repository.UpdateAsync(exercise, cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
