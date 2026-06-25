using ZeroFat.Application.Common.Persistence;
using Microsoft.Extensions.Localization;

namespace ZeroFat.GymUp.Application.Creator.Trainers;
public class ActiveTrainersRequest : ICommand<Result>
{
    public List<DefaultIdType> Ids { get; set; } = [];
    public bool IsActive { get; set; }
}

public class ActiveTrainersRequestHandler(
    IRepository<Trainer> repository,
    IStringLocalizer<ActiveTrainersRequestHandler> localizer) : ICommandHandler<ActiveTrainersRequest, Result>
{

    public async Task<Result> Handle(ActiveTrainersRequest request, CancellationToken cancellationToken)
    {
        foreach (var ingId in request.Ids)
        {
            var trainer = await repository.GetByIdAsync(ingId, cancellationToken);
            if (trainer != null)
            {
                trainer.IsActive = request.IsActive;
            }
        }

        await repository.SaveChangesAsync(cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
