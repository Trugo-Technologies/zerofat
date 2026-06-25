using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Core;

namespace ZeroFat.Application.Core.Banners;
public class GetBannerRequest(DefaultIdType id) : IQuery<Result<BannerDetailsDto>>
{
    public DefaultIdType Id { get; set; } = id;
}

public class GetBannerRequestHandler(IRepositoryWithEvents<Banner> repository, IStringLocalizer<GetBannerRequestHandler> localizer) : IRequestHandler<GetBannerRequest, Result<BannerDetailsDto>>
{
    private readonly IRepositoryWithEvents<Banner> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<BannerDetailsDto>> Handle(GetBannerRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new BannerByIdSpec<BannerDetailsDto>(request.Id), cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["Banner not found", request.Id]);

        return await Result<BannerDetailsDto>.SuccessAsync(entity);
    }
}
