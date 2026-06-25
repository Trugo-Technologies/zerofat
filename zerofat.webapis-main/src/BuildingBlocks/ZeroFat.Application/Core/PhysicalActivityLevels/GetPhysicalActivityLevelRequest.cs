using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Core;

namespace ZeroFat.Application.Core.PhysicalActivityLevels;
public class GetPhysicalActivityLevelRequest(DefaultIdType id) : IQuery<Result<PhysicalActivityLevelDetailsDto>>
{
    public DefaultIdType Id { get; set; } = id;
}

public class GetPhysicalActivityLevelRequestHandler(IRepositoryWithEvents<PhysicalActivityLevel> repository, IStringLocalizer<GetPhysicalActivityLevelRequestHandler> localizer) : IRequestHandler<GetPhysicalActivityLevelRequest, Result<PhysicalActivityLevelDetailsDto>>
{
    private readonly IRepositoryWithEvents<PhysicalActivityLevel> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<PhysicalActivityLevelDetailsDto>> Handle(GetPhysicalActivityLevelRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new PhysicalActivityLevelByIdSpec<PhysicalActivityLevelDetailsDto>(request.Id), cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["PhysicalActivityLevel not found", request.Id]);

        return await Result<PhysicalActivityLevelDetailsDto>.SuccessAsync(entity);
    }
}
