using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.ClientPortal.Domain.ClientManagement;

namespace ZeroFat.ClientPortal.Application.ClientManagement.ClientPaymentMethods;

public class GetClientPaymentMethodRequest : IQuery<Result<ClientPaymentMethodDetailsDto>>
{
    public DefaultIdType Id { get; set; }
    public GetClientPaymentMethodRequest(DefaultIdType id) => Id = id;
}
public class GetClientPaymentMethodRequestHandler : IQueryHandler<GetClientPaymentMethodRequest, Result<ClientPaymentMethodDetailsDto>>
{
    private readonly IRepository<ClientPaymentMethod> _repository;
    private readonly IStripeService _stripeService;
    private readonly IStringLocalizer<GetClientPaymentMethodRequestHandler> _localizer;

    public GetClientPaymentMethodRequestHandler(IRepository<ClientPaymentMethod> repository, IStringLocalizer<GetClientPaymentMethodRequestHandler> localizer, IStripeService stripeService)
    {
        (_repository, _localizer) = (repository, localizer);
        _stripeService = stripeService;
    }

    public async Task<Result<ClientPaymentMethodDetailsDto>> Handle(GetClientPaymentMethodRequest request, CancellationToken cancellationToken)
    {
        var result = await _repository.FirstOrDefaultAsync(new ClientPaymentMethodByIdSpec(request.Id), cancellationToken);
        _ = result ?? throw new NotFoundException(_localizer[$"payment Method not found"]);

        return await Result<ClientPaymentMethodDetailsDto>.SuccessAsync(result);
    }
}
