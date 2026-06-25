using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Enums;
using ZeroFat.ClientPortal.Domain.ClientManagement;

namespace ZeroFat.ClientPortal.Application.ClientManagement.Clients;

public class SetupClientAllergicsRequest : ICommand<Result>
{
    public List<DefaultIdType>? ClientAllergicIds { get; set; }
}

public class SetupClientAllergicsRequestHandler(
    IRepository<Client> repository,
    ICurrentUser currentUser,
    IStringLocalizer<SetupClientAllergicsRequestHandler> localizer) : ICommandHandler<SetupClientAllergicsRequest, Result>
{
    private readonly IRepository<Client> _repository = repository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IStringLocalizer<SetupClientAllergicsRequestHandler> _localizer = localizer;

    public async Task<Result> Handle(SetupClientAllergicsRequest request, CancellationToken cancellationToken)
    {
        bool isClient = _currentUser.GetRoleType()!.Equals(nameof(UserType.Client).ToString(), StringComparison.OrdinalIgnoreCase);
        if (!isClient)
            throw new BadRequestException("only client can register");

        var client = await _repository.GetByIdAsync(_currentUser.GetUserId(), cancellationToken);

        _ = client ?? throw new NotFoundException(_localizer["Client not found"]);

        client.ClientAllergicIds = request.ClientAllergicIds ?? [];


        await _repository.UpdateAsync(client, cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
