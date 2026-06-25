using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using Microsoft.Extensions.Localization;

namespace ZeroFat.GymUp.Application.Creator.Trainers;
public class ActiveTrainerRequest : ICommand<Result>
{
    public DefaultIdType Id { get; set; }
    public ActiveTrainerRequest(DefaultIdType id) => Id = id;
}

public class ActiveTrainerRequestHandler(IRepository<Trainer> repository, IStringLocalizer<ActiveTrainerRequestHandler> localizer) : ICommandHandler<ActiveTrainerRequest, Result>
{
    private readonly IRepository<Trainer> _repository = repository;
    private readonly IStringLocalizer<ActiveTrainerRequestHandler> _localizer = localizer;

    public async Task<Result> Handle(ActiveTrainerRequest request, CancellationToken cancellationToken)
    {
        var trainer = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = trainer ?? throw new NotFoundException(_localizer["Trainer not found"]);


        trainer.IsActive = !trainer.IsActive;

        await _repository.UpdateAsync(trainer, cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
