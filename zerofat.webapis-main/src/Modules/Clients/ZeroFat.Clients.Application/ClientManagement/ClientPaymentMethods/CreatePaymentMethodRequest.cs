using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.ClientManagement.ClientPaymentMethods;

public class CreateClientPaymentMethodRequest : ICommand<Result<DefaultIdType>>
{
    public bool IsDefault { get; set; }
    public string? StripeId { get; set; }
}
public class CreateClientPaymentMethodRequestValidator : CustomValidator<CreateClientPaymentMethodRequest>
{
    public CreateClientPaymentMethodRequestValidator(
        IReadRepository<ClientPaymentMethod> repo,
        IReadRepository<Client> clientRepo,
        IStringLocalizer<CreateClientPaymentMethodRequest> localizer)
    {
        RuleFor(x => x.StripeId)
            .NotEmpty()
            .NotNull();

        // _ = RuleFor(r => r.IsDefault)
        //     .MustAsync(async (request, id, _) => await repo.AnyAsync(new ExpressionSpecification<ClientPaymentMethod>(x => x.IsDefault && x.ClientId == request.ClientId), _))
        //     .WithMessage(localizer[$"Only one payment method can be marked as default for a client."]);
    }
}

public class CreateClientPaymentMethodRequestHandler(
    IRepositoryWithEvents<ClientPaymentMethod> repo,
    ICurrentUser currentUser,
    IStripeService stripeService, IReadRepository<Client> clientRepo, IStringLocalizer<CreateClientPaymentMethodRequestHandler> localizer) : ICommandHandler<CreateClientPaymentMethodRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<ClientPaymentMethod> _repo = repo;
    private readonly IReadRepository<Client> _clientRepo = clientRepo;
    private readonly IStripeService _stripeService = stripeService;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IStringLocalizer<CreateClientPaymentMethodRequestHandler> _localizer = localizer;

    public async Task<Result<DefaultIdType>> Handle(CreateClientPaymentMethodRequest request, CancellationToken cancellationToken)
    {
        bool isClient = _currentUser.GetRoleType()!.Equals(nameof(UserType.Client).ToString(), StringComparison.OrdinalIgnoreCase);
        if (!isClient)
        {
            throw new BadRequestException("only client can register");
        }

        var client = await _clientRepo.GetByIdAsync(_currentUser.GetUserId(), cancellationToken);
        _ = client ?? throw new NotFoundException(_localizer["Client not found"]);

        var paymentMethod = new ClientPaymentMethod
        {
            IsDefault = request.IsDefault,
            StripeId = request.StripeId,
            ClientId = client.Id
        };

        var res = await _stripeService.AttachPaymentMethodToCustomer(paymentMethod.StripeId!, client.StripeId!);
        if (res == null)
            throw new BadRequestException(_localizer["Something went wrong"]);

        paymentMethod.Type = res.Type;
        paymentMethod.CustomerId = res.CustomerId;
        paymentMethod.CardBrand = res.Card?.Brand;
        paymentMethod.CardExpMonth = res.Card?.ExpMonth;
        paymentMethod.CardFunding = res.Card?.Funding;
        paymentMethod.CardLast4 = res.Card?.Last4;
        paymentMethod.CardName = res.Card?.Name;

        var defaultCard = await _repo.FirstOrDefaultAsync(new ExpressionSpecification<ClientPaymentMethod>(x => x.ClientId == client.Id && x.IsDefault), cancellationToken);
        if (paymentMethod.IsDefault && defaultCard != null)
        {
            defaultCard.IsDefault = false;
        }
        else
        {
            paymentMethod.IsDefault = true;
        }

        if(paymentMethod.IsDefault)
        {
            var success = await _stripeService.MakeDefaultPaymentMethod(paymentMethod.StripeId!, client.StripeId!);
            if (!success)
                throw new BadRequestException(_localizer["Something went wrong"]);
        }

        await _repo.AddAsync(paymentMethod, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(paymentMethod.Id);
    }
}
