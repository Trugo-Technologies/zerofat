using ZeroFat.Application.Common.Persistence;
using Microsoft.Extensions.Localization;

namespace ZeroFat.GymUp.Application.Catalog.BodyParts;
public class ActiveBodyPartsRequest : ICommand<Result>
{
    public List<DefaultIdType> Ids { get; set; } = [];
    public bool IsActive { get; set; }
}

public class ActiveBodyPartsRequestHandler(IRepository<BodyPart> repository, IStringLocalizer<ActiveBodyPartsRequestHandler> localizer) : ICommandHandler<ActiveBodyPartsRequest, Result>
{

    public async Task<Result> Handle(ActiveBodyPartsRequest request, CancellationToken cancellationToken)
    {
        foreach (var ingId in request.Ids)
        {
            var bodyPart = await repository.GetByIdAsync(ingId, cancellationToken);
            if (bodyPart != null)
            {
                bodyPart.IsActive = request.IsActive;
            }
        }

        await repository.SaveChangesAsync(cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
