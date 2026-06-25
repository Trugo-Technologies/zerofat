using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.ClientPortal.Domain.ClientManagement;

namespace ZeroFat.ClientPortal.Application.ClientManagement.ClientChats;
public class CreateClientChatRequest : ICommand<Result<DefaultIdType>>
{
    public DateTime? ChatDate { get; set; }
    public string? ChatId { get; set; }
    public string? ChatType { get; set; }
    public string? BaseDeviceId { get; set; }
    public string? ChannelId { get; set; }
    public string? ChannelName { get; set; }
}

public class CreateClientChatRequestValidator : CustomValidator<CreateClientChatRequest>
{
    public CreateClientChatRequestValidator(IReadRepository<ClientChat> repository, IReadRepository<Client> clientRepo, IStringLocalizer<CreateClientChatRequestValidator> loc)
    {

    }
}


public class CreateClientChatRequestHandler(IRepository<ClientChat> repo, ICurrentUser currentUser) : IRequestHandler<CreateClientChatRequest, Result<DefaultIdType>>
{
    private readonly IRepository<ClientChat> _repo = repo;
    private readonly ICurrentUser _currentUser = currentUser;


    public async Task<Result<DefaultIdType>> Handle(CreateClientChatRequest request, CancellationToken cancellationToken)
    {
        //bool isClient = _currentUser.GetRoleType()!.Equals(nameof(UserType.Client).ToString(), StringComparison.OrdinalIgnoreCase);
        //if (!isClient)
        //{
        //    throw new BadRequestException("only client can register");
        //}

        var chat = await _repo.FirstOrDefaultAsync(new ExpressionSpecification<ClientChat>(x => x.ChatId == request.ChatId && (x.ClientId == _currentUser.GetUserId() || (x.BaseDeviceId == request.BaseDeviceId && !string.IsNullOrWhiteSpace(request.BaseDeviceId)))), cancellationToken);
        if (chat != null)
        {
            return await Result<DefaultIdType>.SuccessAsync(chat.Id);
        }
        var entity = new ClientChat
        {
            ClientId = _currentUser.GetUserId(),
            ChatId = request.ChatId,
            BaseDeviceId = request.BaseDeviceId,
            DeviceId = _currentUser.GetDeviceId(),
            ChatDate = request.ChatDate.HasValue ? DateTime.SpecifyKind(request.ChatDate.Value, DateTimeKind.Utc) : null,
            ChatType = request.ChatType,
            ChannelId = request.ChannelId,
            ChannelName = request.ChannelName,
        };

        await _repo.AddAsync(entity, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(entity.Id);
    }

}
