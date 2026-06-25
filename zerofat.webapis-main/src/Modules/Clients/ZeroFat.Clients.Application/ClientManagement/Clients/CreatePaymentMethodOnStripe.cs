using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using Microsoft.Extensions.Localization;
using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.Application.Common.Interfaces;

namespace ZeroFat.ClientPortal.Application.ClientManagement.Clients;

public class CreatePaymentMethodOnStripe : ICommand<Result<string>>
{
    
}

public class CreatePaymentMethodOnStripeHandler(
    IRepository<Client> repository,
    ICurrentUser currentUser,
    IStripeService stripeService,
    IStringLocalizer<CreatePaymentMethodOnStripeHandler> localizer) : ICommandHandler<CreatePaymentMethodOnStripe, Result<string>>
{
    private readonly IRepository<Client> _repository = repository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IStripeService _stripeService = stripeService;
    private readonly IStringLocalizer<CreatePaymentMethodOnStripeHandler> _localizer = localizer;

    public async Task<Result<string>> Handle(CreatePaymentMethodOnStripe request, CancellationToken cancellationToken)
    {
        var client = await _repository.GetByIdAsync(_currentUser.GetUserId(), cancellationToken);
        _ = client ?? throw new NotFoundException(_localizer["Client not found"]);
        if(client.StripeId == null)
            throw new BadRequestException(_localizer["Invalid client Info"]);

        var clientSecret = await _stripeService.CreateSetupIntentAsync(client.StripeId);

        return await Result<string>.SuccessAsync(data: clientSecret);
    }
}
