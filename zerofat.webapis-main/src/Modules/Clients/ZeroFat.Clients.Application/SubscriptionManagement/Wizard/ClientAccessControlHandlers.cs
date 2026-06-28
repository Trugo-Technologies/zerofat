using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Validation;
using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.Wizard;

public class BlockClientRequest : ICommand<Result<ClientAccessControlDto>>
{
    public DefaultIdType ClientId { get; set; }
    public ClientBlockOption BlockOption { get; set; }
    public DateTime? BlockedUntil { get; set; }
}

public class BlockClientRequestValidator : CustomValidator<BlockClientRequest>
{
    public BlockClientRequestValidator(IStringLocalizer<BlockClientRequestValidator> localizer)
    {
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.BlockOption)
            .Must(x => x is ClientBlockOption.OneDay or ClientBlockOption.Custom or ClientBlockOption.Permanent)
            .WithMessage(localizer["A valid block option is required."]);

        RuleFor(x => x.BlockedUntil)
            .NotNull()
            .When(x => x.BlockOption == ClientBlockOption.Custom)
            .WithMessage(localizer["Blocked until date and time is required for custom blocks."]);

        RuleFor(x => x.BlockedUntil)
            .Must(x => x.HasValue && x.Value > DateTime.UtcNow)
            .When(x => x.BlockOption == ClientBlockOption.Custom)
            .WithMessage(localizer["Blocked until date and time must be in the future."]);
    }
}

public class BlockClientRequestHandler(
    ICurrentUser currentUser,
    IRepository<Client> clientRepo,
    IRepository<ClientAccountActivityLog> activityLogRepo,
    IStringLocalizer<BlockClientRequestHandler> localizer) : ICommandHandler<BlockClientRequest, Result<ClientAccessControlDto>>
{
    public async Task<Result<ClientAccessControlDto>> Handle(BlockClientRequest request, CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);

        var client = await clientRepo.GetByIdAsync(request.ClientId, cancellationToken)
            ?? throw new NotFoundException(localizer["Client not found"]);

        var utcNow = DateTime.UtcNow;
        var previousValue = ClientAccessBlockHelper.GetBlockDescription(client.BlockOption, client.BlockedUntil);

        client.BlockedOn = utcNow;
        client.BlockOption = request.BlockOption;
        client.BlockedUntil = request.BlockOption switch
        {
            ClientBlockOption.OneDay => utcNow.AddDays(1),
            ClientBlockOption.Custom => request.BlockedUntil,
            ClientBlockOption.Permanent => null,
            _ => null
        };

        await clientRepo.UpdateAsync(client, cancellationToken);

        var newValue = ClientAccessBlockHelper.GetBlockDescription(client.BlockOption, client.BlockedUntil);
        await ClientAccountActivityLogHelper.LogAsync(
            activityLogRepo,
            currentUser,
            request.ClientId,
            ClientAccountActivityAction.ClientBlocked,
            previousValue,
            newValue,
            cancellationToken: cancellationToken);

        return await Result<ClientAccessControlDto>.SuccessAsync(
            BuildAccessControlDto(client.BlockOption, client.BlockedOn, client.BlockedUntil));
    }

    internal static ClientAccessControlDto BuildAccessControlDto(
        ClientBlockOption blockOption,
        DateTime? blockedOn,
        DateTime? blockedUntil)
    {
        var utcNow = DateTime.UtcNow;
        var isBlocked = ClientAccessBlockHelper.IsBlocked(blockOption, blockedUntil, utcNow);

        return new ClientAccessControlDto
        {
            AccessStatus = isBlocked ? "Blocked" : "Active",
            BlockOption = blockOption,
            BlockedOn = blockedOn,
            BlockedUntil = blockedUntil
        };
    }

    internal static ClientAccessControlDto BuildAccessControlDto(Client client)
        => BuildAccessControlDto(client.BlockOption, client.BlockedOn, client.BlockedUntil);
}

public class UnblockClientRequest(DefaultIdType clientId) : ICommand<Result<ClientAccessControlDto>>
{
    public DefaultIdType ClientId { get; set; } = clientId;
}

public class UnblockClientRequestHandler(
    ICurrentUser currentUser,
    IRepository<Client> clientRepo,
    IRepository<ClientAccountActivityLog> activityLogRepo,
    IStringLocalizer<UnblockClientRequestHandler> localizer) : ICommandHandler<UnblockClientRequest, Result<ClientAccessControlDto>>
{
    public async Task<Result<ClientAccessControlDto>> Handle(UnblockClientRequest request, CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);

        var client = await clientRepo.GetByIdAsync(request.ClientId, cancellationToken)
            ?? throw new NotFoundException(localizer["Client not found"]);

        var previousValue = ClientAccessBlockHelper.GetBlockDescription(client.BlockOption, client.BlockedUntil);
        ClientAccessBlockHelper.ClearBlock(client);
        client.BlockedOn = null;

        await clientRepo.UpdateAsync(client, cancellationToken);

        await ClientAccountActivityLogHelper.LogAsync(
            activityLogRepo,
            currentUser,
            request.ClientId,
            ClientAccountActivityAction.ClientUnblocked,
            previousValue,
            "Active",
            cancellationToken: cancellationToken);

        return await Result<ClientAccessControlDto>.SuccessAsync(BlockClientRequestHandler.BuildAccessControlDto(client));
    }
}
