using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Core;

namespace ZeroFat.Application.Core.Banners;
public class ActiveBannerRequest : ICommand<Result>
{
    public DefaultIdType Id { get; set; }
    public ActiveBannerRequest(DefaultIdType id) => Id = id;
}

public class ActiveBannerRequestHandler(IRepository<Banner> repository, IStringLocalizer<ActiveBannerRequestHandler> localizer) : ICommandHandler<ActiveBannerRequest, Result>
{
    private readonly IRepository<Banner> _repository = repository;
    private readonly IStringLocalizer<ActiveBannerRequestHandler> _localizer = localizer;

    public async Task<Result> Handle(ActiveBannerRequest request, CancellationToken cancellationToken)
    {
        var adv = await _repository.GetByIdAsync(request.Id, cancellationToken);
        _ = adv ?? throw new NotFoundException(_localizer["Banner not found"]);

        adv.IsActive = !adv.IsActive;

        await _repository.UpdateAsync(adv, cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
