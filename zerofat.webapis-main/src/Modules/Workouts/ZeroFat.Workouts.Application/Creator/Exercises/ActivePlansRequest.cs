using ZeroFat.Application.Common.Persistence;
using Microsoft.Extensions.Localization;

namespace ZeroFat.GymUp.Application.Creator.Exercises;
public class ActiveExercisesRequest : ICommand<Result>
{
    public List<DefaultIdType> Ids { get; set; } = [];
    public bool IsActive { get; set; }
}

public class ActiveExercisesRequestHandler(
    IRepository<Exercise> repository,
    IStringLocalizer<ActiveExercisesRequestHandler> localizer) : ICommandHandler<ActiveExercisesRequest, Result>
{

    public async Task<Result> Handle(ActiveExercisesRequest request, CancellationToken cancellationToken)
    {
        foreach (var ingId in request.Ids)
        {
            var exercise = await repository.GetByIdAsync(ingId, cancellationToken);
            if (exercise != null)
            {
                exercise.IsActive = request.IsActive;
            }
        }

        await repository.SaveChangesAsync(cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
