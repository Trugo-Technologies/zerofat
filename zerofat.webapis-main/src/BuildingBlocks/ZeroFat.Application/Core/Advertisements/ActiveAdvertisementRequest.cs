using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Core;

namespace ZeroFat.Application.Core.Advertisements;
public class ActiveAdvertisementRequest : ICommand<Result>
{
    public DefaultIdType Id { get; set; }
    public ActiveAdvertisementRequest(DefaultIdType id) => Id = id;
}

public class ActiveAdvertisementRequestHandler(IRepository<Advertisement> repository, IStringLocalizer<ActiveAdvertisementRequestHandler> localizer) : ICommandHandler<ActiveAdvertisementRequest, Result>
{
    private readonly IRepository<Advertisement> _repository = repository;
    private readonly IStringLocalizer<ActiveAdvertisementRequestHandler> _localizer = localizer;

    public async Task<Result> Handle(ActiveAdvertisementRequest request, CancellationToken cancellationToken)
    {
        var adv = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = adv ?? throw new NotFoundException(_localizer["Advertisement not found"]);


        adv.IsActive = !adv.IsActive;

        await _repository.UpdateAsync(adv, cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
