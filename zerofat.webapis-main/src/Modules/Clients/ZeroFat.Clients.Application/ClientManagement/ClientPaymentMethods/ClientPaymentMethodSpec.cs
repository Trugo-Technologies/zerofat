using Ardalis.Specification;
using ZeroFat.ClientPortal.Domain.ClientManagement;

namespace ZeroFat.ClientPortal.Application.ClientManagement.ClientPaymentMethods;

public class ClientPaymentMethodByIdSpec : Specification<ClientPaymentMethod, ClientPaymentMethodDetailsDto>
{
    public ClientPaymentMethodByIdSpec(DefaultIdType id)
    {
        Query.Where(p => p.Id == id);
    }
}


public class DefaultClientPaymentMethodByIdSpec<T> : Specification<ClientPaymentMethod, T>
{
    public DefaultClientPaymentMethodByIdSpec(DefaultIdType clientId)
    {
        Query.Where(p => p.ClientId == clientId && p.IsDefault);
    }
}


