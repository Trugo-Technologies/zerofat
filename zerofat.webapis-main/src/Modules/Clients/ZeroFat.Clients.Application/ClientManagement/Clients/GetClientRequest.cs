using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.ClientPortal.Domain.ClientManagement;

namespace ZeroFat.ClientPortal.Application.ClientManagement.Clients;

public class GetClientRequest(DefaultIdType id) : IQuery<Result<ClientDetailsDto>>
{
    public DefaultIdType Id { get; set; } = id;
}

public class GetClientRequestHandler(IRepositoryWithEvents<Client> repository, IStringLocalizer<GetClientRequestHandler> localizer) : IRequestHandler<GetClientRequest, Result<ClientDetailsDto>>
{
    private readonly IRepositoryWithEvents<Client> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<ClientDetailsDto>> Handle(GetClientRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new ClientByIdSpec<ClientDetailsDto>(request.Id), cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["Client not found", request.Id]);

        return await Result<ClientDetailsDto>.SuccessAsync(entity);
    }

}
