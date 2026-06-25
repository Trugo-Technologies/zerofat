using MediatR;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.ClientManagement.ClientChats;
public class SearchClientChatsRequest : PaginationFilter, IQuery<PaginationResponse<ClientChatDto>>
{
    public DefaultIdType? ClientId { get; set; }
    public string? BaseDeviceId { get; set; }

}


public class SearchClientChatsRequestHandler(IReadRepository<ClientChat> repository, ICurrentUser currentUser ) : IRequestHandler<SearchClientChatsRequest, PaginationResponse<ClientChatDto>>
{
    private readonly IReadRepository<ClientChat> _repository = repository;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<PaginationResponse<ClientChatDto>> Handle(SearchClientChatsRequest request, CancellationToken cancellationToken)
    {
        bool isClient = _currentUser.GetRoleType()!.Equals(nameof(UserType.Client).ToString(), StringComparison.OrdinalIgnoreCase);
        if (isClient)
        {
            request.ClientId = _currentUser.GetUserId();
        }

        return await _repository.PaginatedListAsync(new ClientChatsBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);
    }

}
