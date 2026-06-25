using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Enums;
using Microsoft.AspNetCore.Http;
using ZeroFat.ClientPortal.Domain;
using ZeroFat.ClientPortal.Domain.ClientManagement;

namespace ZeroFat.ClientPortal.Application.ClientManagement.Clients;

public class SetupClientNameRequest : ICommand<Result>
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public IFormFile? Image { get; set; }
}

public class SetupClientNameRequestHandler(
    IRepository<Client> repository,
    IFileStorageManager uploadFile,
    IStripeService stripeService,
    ICurrentUser currentUser, IStringLocalizer<SetupClientNameRequestHandler> localizer) : ICommandHandler<SetupClientNameRequest, Result>
{
    private readonly IRepository<Client> _repository = repository;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IStripeService _stripeService = stripeService;
    private readonly IFileStorageManager _uploadFile = uploadFile;
    private readonly IStringLocalizer<SetupClientNameRequestHandler> _localizer = localizer;

    public async Task<Result> Handle(SetupClientNameRequest request, CancellationToken cancellationToken)
    {
        bool isClient = _currentUser.GetRoleType()!.Equals(nameof(UserType.Client).ToString(), StringComparison.OrdinalIgnoreCase);
        if (!isClient)
        {
            throw new BadRequestException("only client can register");
        }

        var client = await _repository.GetByIdAsync(_currentUser.GetUserId(), cancellationToken);
        _ = client ?? throw new NotFoundException(_localizer["Client not found"]);

        if (request.FullName.HasValue())
        {
            client.FullName = request.FullName;
        }

        if (request.Email.HasValue())
        {
            client.Email = request.Email;
        }

        if (request.Image != null)
        {
            client.ImageUrl = await _uploadFile.UploadAsync<Client>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
        }

        if (client.StripeId.HasValue())
        {
            await _stripeService.UpdateCustomerOnStripe(client.StripeId!, client.Email, client.FullName, client.Mobile, client.Id.ToString());
        }
        else
        {
            client.StripeId = await _stripeService.CreateCustomerOnStripe(client.Email, client.FullName, client.Mobile, client.Id.ToString());
        }

        await _repository.UpdateAsync(client, cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
