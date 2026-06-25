using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Core;

namespace ZeroFat.GymUp.Application.Catalog.BodyParts;

public class GetBodyPartRequest(DefaultIdType id) : IQuery<Result<BodyPartDetailsDto>>
{
    public DefaultIdType Id { get; set; } = id;
}

public class GetBodyPartRequestHandler(IReadRepository<BodyPart> repository, IStringLocalizer<GetBodyPartRequestHandler> localizer) : IRequestHandler<GetBodyPartRequest, Result<BodyPartDetailsDto>>
{
    private readonly IReadRepository<BodyPart> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<BodyPartDetailsDto>> Handle(GetBodyPartRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new BodyPartByIdSpec<BodyPartDetailsDto>(request.Id), cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["BodyPart not found", request.Id]);

        return await Result<BodyPartDetailsDto>.SuccessAsync(entity);
    }

}
