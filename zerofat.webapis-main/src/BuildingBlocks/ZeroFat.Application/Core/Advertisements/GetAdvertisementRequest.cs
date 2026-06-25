using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Core;

namespace ZeroFat.Application.Core.Advertisements;
public class GetAdvertisementRequest(DefaultIdType id) : IQuery<Result<AdvertisementDetailsDto>>
{
    public DefaultIdType Id { get; set; } = id;
}

public class GetAdvertisementRequestHandler(IRepositoryWithEvents<Advertisement> repository, IStringLocalizer<GetAdvertisementRequestHandler> localizer) : IRequestHandler<GetAdvertisementRequest, Result<AdvertisementDetailsDto>>
{
    private readonly IRepositoryWithEvents<Advertisement> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<AdvertisementDetailsDto>> Handle(GetAdvertisementRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new AdvertisementByIdSpec<AdvertisementDetailsDto>(request.Id), cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["Advertisement not found", request.Id]);

        return await Result<AdvertisementDetailsDto>.SuccessAsync(entity);
    }
}
