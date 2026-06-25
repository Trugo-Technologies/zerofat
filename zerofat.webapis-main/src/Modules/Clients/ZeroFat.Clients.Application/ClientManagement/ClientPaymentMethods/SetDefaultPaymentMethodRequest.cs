using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Domain.ClientManagement;

namespace ZeroFat.ClientPortal.Application.ClientManagement.ClientPaymentMethods;

public class SetDefaultPaymentMethodRequest : ICommand<Result<bool>>
{
    public DefaultIdType Id { get; set; }
    public bool IsDefault { get; set; }
}
public class SetDefaultPaymentMethodRequestHandler : ICommandHandler<SetDefaultPaymentMethodRequest, Result<bool>>
{
    private readonly IRepositoryWithEvents<ClientPaymentMethod> _repository;
    private readonly IReadRepository<Client> _clientRepo;
    private readonly IStripeService _stripeService;
    private readonly ICurrentUser _currentUser;
    private readonly IStringLocalizer<SetDefaultPaymentMethodRequestHandler> _localizer;

    public SetDefaultPaymentMethodRequestHandler(
        IRepositoryWithEvents<ClientPaymentMethod> repository,
        ICurrentUser currentUser,
        IStringLocalizer<SetDefaultPaymentMethodRequestHandler> localizer, IStripeService stripeService, IReadRepository<Client> clientRepo)
    {
        _repository = repository;
        _localizer = localizer;
        _stripeService = stripeService;
        _currentUser = currentUser;
        _clientRepo = clientRepo;
    }

    public async Task<Result<bool>> Handle(SetDefaultPaymentMethodRequest request, CancellationToken cancellationToken)
    {
        var paymentMethod = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(_localizer["Payment method not found"]);

        if(_currentUser.GetUserId() != paymentMethod.ClientId)
            throw new BadRequestException(_localizer["Payment method not your"]);

        paymentMethod.IsDefault = request.IsDefault;

        var defaultCard = await _repository.FirstOrDefaultAsync(new ExpressionSpecification<ClientPaymentMethod>(x => x.ClientId == paymentMethod.ClientId && x.IsDefault), cancellationToken);
        if (defaultCard is not null)
            defaultCard.IsDefault = false;

        var res = await _stripeService.MakeDefaultPaymentMethod(paymentMethod.StripeId!, paymentMethod.CustomerId!);
        if (!res)
            throw new BadRequestException(_localizer["Something went wrong"]);

        await _repository.UpdateAsync(paymentMethod, cancellationToken);

        return await Result<bool>.SuccessAsync();
    }
}

