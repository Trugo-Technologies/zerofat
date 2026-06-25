using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using Microsoft.Extensions.Localization;

namespace ZeroFat.GymUp.Application.Catalog.BodyParts;
public class ActiveBodyPartRequest : ICommand<Result>
{
    public DefaultIdType Id { get; set; }
    public ActiveBodyPartRequest(DefaultIdType id) => Id = id;
}

public class ActiveBodyPartRequestHandler(IRepository<BodyPart> repository, IStringLocalizer<ActiveBodyPartRequestHandler> localizer) : ICommandHandler<ActiveBodyPartRequest, Result>
{
    private readonly IRepository<BodyPart> _repository = repository;
    private readonly IStringLocalizer<ActiveBodyPartRequestHandler> _localizer = localizer;

    public async Task<Result> Handle(ActiveBodyPartRequest request, CancellationToken cancellationToken)
    {
        var part = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = part ?? throw new NotFoundException(_localizer["Body part not found"]);


        part.IsActive = !part.IsActive;

        await _repository.UpdateAsync(part, cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
