using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using Microsoft.Extensions.Localization;

namespace ZeroFat.GymUp.Application.Catalog.WorkoutTypes;
public class ActiveWorkoutTypeRequest : ICommand<Result>
{
    public DefaultIdType Id { get; set; }
    public ActiveWorkoutTypeRequest(DefaultIdType id)
    {
        Id = id;
    }
}

public class ActiveWorkoutTypeRequestHandler(IRepository<WorkoutType> repository, IStringLocalizer<ActiveWorkoutTypeRequestHandler> localizer) : ICommandHandler<ActiveWorkoutTypeRequest, Result>
{
    private readonly IRepository<WorkoutType> _repository = repository;
    private readonly IStringLocalizer<ActiveWorkoutTypeRequestHandler> _localizer = localizer;

    public async Task<Result> Handle(ActiveWorkoutTypeRequest request, CancellationToken cancellationToken)
    {
        var type = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = type ?? throw new NotFoundException(_localizer["Type not found"]);


        type.IsActive = !type.IsActive;

        await _repository.UpdateAsync(type, cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
