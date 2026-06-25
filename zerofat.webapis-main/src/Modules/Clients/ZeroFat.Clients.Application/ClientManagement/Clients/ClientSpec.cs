using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Application.ClientManagement.Clients;
using ZeroFat.ClientPortal.Domain.ClientManagement;

namespace ZeroFat.ClientPortal.Application.ClientManagement;

public class ClientsBySearchRequestSpec : EntitiesByPaginationFilterSpec<Client, ClientDto>
{
    public ClientsBySearchRequestSpec(SearchClientsRequest request)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
             .Where(x => x.Gender == request.Gender, request.Gender.HasValue)
             .Where(x => x.IsActive == request.IsActive, request.IsActive.HasValue);
    }
}

public class ClientByIdSpec<T> : Specification<Client, T>
{
    public ClientByIdSpec(Guid id)
    {
        Query.Where(p => p.Id == id);
    }
}

public class ClientByIdSpec : Specification<Client>
{
    public ClientByIdSpec(Guid id)
    {
        Query.Where(p => p.Id == id);
    }
}


public class ClientAllergicsByIdSpec : Specification<Client, List<DefaultIdType>>
{
    public ClientAllergicsByIdSpec(Guid id)
    {
        Query.Where(p => p.Id == id).Select(x=>x.ClientAllergicIds);
    }
}



public class ClientStatusByIdSpec : Specification<Client, ClientStatusDto>
{
    public ClientStatusByIdSpec(Guid id)
    {
        Query.Where(p => p.Id == id);
    }
}
