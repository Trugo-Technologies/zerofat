using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Core;

namespace ZeroFat.Application.Core.Advertisements;

public class DeleteAdvertisementRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType Id { get; set; }
    public DeleteAdvertisementRequest(DefaultIdType id) => Id = id;
}


public class DeleteAdvertisementRequestHandler(IRepository<Advertisement> repository, IStringLocalizer<DeleteAdvertisementRequestHandler> localizer) : IRequestHandler<DeleteAdvertisementRequest, Result<DefaultIdType>>
{
    private readonly IRepository<Advertisement> _repository = repository;
    private readonly IStringLocalizer<DeleteAdvertisementRequestHandler> _localizer = localizer;

    public async Task<Result<DefaultIdType>> Handle(DeleteAdvertisementRequest request, CancellationToken cancellationToken)
    {
        var adv = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = adv ?? throw new NotFoundException(_localizer["Advertisement not found"]);


        await _repository.DeleteAsync(adv, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(adv.Id);
    }

}
