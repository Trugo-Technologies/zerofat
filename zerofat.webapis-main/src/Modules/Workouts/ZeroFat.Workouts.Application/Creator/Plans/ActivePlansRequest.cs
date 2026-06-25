using ZeroFat.Application.Common.Persistence;
using Microsoft.Extensions.Localization;

namespace ZeroFat.GymUp.Application.Creator.Plans;
public class ActivePlansRequest : ICommand<Result>
{
    public List<DefaultIdType> Ids { get; set; } = [];
    public bool IsActive { get; set; }
}

public class ActivePlansRequestHandler(
    IRepository<Plan> repository,
    IStringLocalizer<ActivePlansRequestHandler> localizer) : ICommandHandler<ActivePlansRequest, Result>
{

    public async Task<Result> Handle(ActivePlansRequest request, CancellationToken cancellationToken)
    {
        foreach (var ingId in request.Ids)
        {
            var plan = await repository.GetByIdAsync(ingId, cancellationToken);
            if (plan != null)
            {
                plan.IsActive = request.IsActive;
            }
        }

        await repository.SaveChangesAsync(cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
