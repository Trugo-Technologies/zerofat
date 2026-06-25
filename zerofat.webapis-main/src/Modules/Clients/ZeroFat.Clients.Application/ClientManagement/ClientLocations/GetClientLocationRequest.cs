using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.ClientPortal.Domain.ClientManagement;

namespace ZeroFat.ClientPortal.Application.ClientManagement.ClientLocations;

public class GetClientLocationRequest(DefaultIdType id) : IQuery<Result<ClientLocationDetailsDto>>
{
    public DefaultIdType Id { get; set; } = id;
}

public class GetClientLocationRequestHandler(IRepositoryWithEvents<ClientLocation> repository, IStringLocalizer<GetClientLocationRequestHandler> localizer) : IRequestHandler<GetClientLocationRequest, Result<ClientLocationDetailsDto>>
{
    private readonly IRepositoryWithEvents<ClientLocation> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<ClientLocationDetailsDto>> Handle(GetClientLocationRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new ClientLocationByIdSpec<ClientLocationDetailsDto>(request.Id), cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["Location not found", request.Id]);

        return await Result<ClientLocationDetailsDto>.SuccessAsync(entity);
    }

}
