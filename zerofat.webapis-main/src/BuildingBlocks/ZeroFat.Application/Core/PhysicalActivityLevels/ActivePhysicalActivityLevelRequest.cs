using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Core;

namespace ZeroFat.Application.Core.PhysicalActivityLevels;
public class ActivePhysicalActivityLevelRequest : ICommand<Result>
{
    public Guid Id { get; set; }
    public ActivePhysicalActivityLevelRequest(Guid id) => Id = id;
}

public class ActivePhysicalActivityLevelRequestHandler(IRepository<PhysicalActivityLevel> repository, IStringLocalizer<ActivePhysicalActivityLevelRequestHandler> localizer) : ICommandHandler<ActivePhysicalActivityLevelRequest, Result>
{
    private readonly IRepository<PhysicalActivityLevel> _repository = repository;
    private readonly IStringLocalizer<ActivePhysicalActivityLevelRequestHandler> _localizer = localizer;

    public async Task<Result> Handle(ActivePhysicalActivityLevelRequest request, CancellationToken cancellationToken)
    {
        var activity = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = activity ?? throw new NotFoundException(_localizer["Activity not found"]);


        activity.IsActive = !activity.IsActive;

        await _repository.UpdateAsync(activity, cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
