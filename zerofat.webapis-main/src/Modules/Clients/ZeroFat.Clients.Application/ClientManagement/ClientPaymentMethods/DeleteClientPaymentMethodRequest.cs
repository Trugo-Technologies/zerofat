using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.ClientPortal.Domain.ClientManagement;

namespace ZeroFat.ClientPortal.Application.ClientManagement.ClientPaymentMethods;

public class DeleteClientPaymentMethodRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType Id { get; set; }
    public DeleteClientPaymentMethodRequest(DefaultIdType id)
    {
        Id = id;
    }
}
public class DeleteClientPaymentMethodRequestHandler(IRepositoryWithEvents<ClientPaymentMethod> repo, IStringLocalizer<DeleteClientPaymentMethodRequestHandler> localizer, IStripeService stripeService) : ICommandHandler<DeleteClientPaymentMethodRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<ClientPaymentMethod> _repo = repo;
    private readonly IStripeService _stripeService = stripeService;
    private readonly IStringLocalizer<DeleteClientPaymentMethodRequestHandler> _localizer = localizer;

    public async Task<Result<DefaultIdType>> Handle(DeleteClientPaymentMethodRequest request, CancellationToken cancellationToken)
    {
        var paymentMethod = await _repo.GetByIdAsync(request.Id, cancellationToken);
        _ = paymentMethod ?? throw new NotFoundException(_localizer["payment Method not found"]);

        var res = await _stripeService.DetachPaymentMethodFromCustomer(paymentMethod.StripeId!);
        if (!res)
            throw new BadRequestException(_localizer["Something went wrong"]);

        await _repo.DeleteAsync(paymentMethod, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(paymentMethod.Id);
    }
}

