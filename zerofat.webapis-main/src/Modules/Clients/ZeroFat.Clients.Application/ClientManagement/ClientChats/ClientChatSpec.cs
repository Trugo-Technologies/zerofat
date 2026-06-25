using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Domain.ClientManagement;

namespace ZeroFat.ClientPortal.Application.ClientManagement.ClientChats;
public class ClientChatsBySearchRequestSpec : EntitiesByPaginationFilterSpec<ClientChat, ClientChatDto>
{
    public ClientChatsBySearchRequestSpec(SearchClientChatsRequest request)
        : base(request)
    {
        Query.Where(x => x.ClientId == request.ClientId, request.ClientId.HasValue)
              .Where(x => x.BaseDeviceId == request.BaseDeviceId, request.BaseDeviceId.HasValue())
             .OrderByDescending(x => x.ChatDate);
    }
}

public class ClientChatByIdSpec<T> : Specification<ClientChat, T>
{
    public ClientChatByIdSpec(DefaultIdType id)
    {
        Query.Where(p => p.Id == id);
    }
}

public class CurrentClientChatByIdSpec<T> : Specification<ClientChat, T>
{
    public CurrentClientChatByIdSpec(DefaultIdType clientId)
    {
        Query.Where(p => p.ClientId == clientId);
    }
}
