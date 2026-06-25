using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using Microsoft.Extensions.Localization;
using ZeroFat.ClientPortal.Domain.ClientManagement;

namespace ZeroFat.ClientPortal.Application.ClientManagement.Clients;

public class ActiveClientRequest : ICommand<Result>
{
    public DefaultIdType Id { get; set; }
    public ActiveClientRequest(DefaultIdType id)
    {
        Id = id;
    }
}

public class ActiveClientRequestHandler(IRepository<Client> repository, IStringLocalizer<ActiveClientRequestHandler> localizer) : ICommandHandler<ActiveClientRequest, Result>
{
    private readonly IRepository<Client> _repository = repository;
    private readonly IStringLocalizer<ActiveClientRequestHandler> _localizer = localizer;

    public async Task<Result> Handle(ActiveClientRequest request, CancellationToken cancellationToken)
    {
        var client = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = client ?? throw new NotFoundException(_localizer["Client not found"]);

        client.IsActive = !client.IsActive;

        await _repository.UpdateAsync(client, cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
