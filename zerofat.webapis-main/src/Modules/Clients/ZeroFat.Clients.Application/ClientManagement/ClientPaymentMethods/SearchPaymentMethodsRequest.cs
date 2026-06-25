using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Domain.ClientManagement;

namespace ZeroFat.ClientPortal.Application.ClientManagement.ClientPaymentMethods;

public class SearchClientPaymentMethodsRequest : IQuery<List<ClientPaymentMethodDto>>
{
}
public class SearchClientPaymentMethodsRequestHandler : IQueryHandler<SearchClientPaymentMethodsRequest, List<ClientPaymentMethodDto>>
{
    private readonly IReadRepository<ClientPaymentMethod> _repository;
    private readonly ICurrentUser _currentUser;
    private readonly IStringLocalizer<SearchClientPaymentMethodsRequestHandler> _localizer;

    public SearchClientPaymentMethodsRequestHandler(IReadRepository<ClientPaymentMethod> repository, IStringLocalizer<SearchClientPaymentMethodsRequestHandler> localizer, ICurrentUser currentUser)
    {
        _repository = repository;
        _localizer = localizer;
        _currentUser = currentUser;
    }

    public async Task<List<ClientPaymentMethodDto>> Handle(SearchClientPaymentMethodsRequest request, CancellationToken cancellationToken)
    {
        var result = await _repository.ListAsync(new ExpressionSpecificationProjecting<ClientPaymentMethod, ClientPaymentMethodDto>(x => x.ClientId == _currentUser.GetUserId()), cancellationToken);

        return result;
    }
}

