using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Core;

namespace ZeroFat.Application.Core.Banners;

public class DeleteBannerRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType Id { get; set; }
    public DeleteBannerRequest(DefaultIdType id) => Id = id;
}


public class DeleteBannerRequestHandler(IRepository<Banner> repository, IStringLocalizer<DeleteBannerRequestHandler> localizer) : IRequestHandler<DeleteBannerRequest, Result<DefaultIdType>>
{
    private readonly IRepository<Banner> _repository = repository;
    private readonly IStringLocalizer<DeleteBannerRequestHandler> _localizer = localizer;

    public async Task<Result<DefaultIdType>> Handle(DeleteBannerRequest request, CancellationToken cancellationToken)
    {
        var adv = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = adv ?? throw new NotFoundException(_localizer["Banner not found"]);


        await _repository.DeleteAsync(adv, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(adv.Id);
    }

}
