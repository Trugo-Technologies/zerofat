using Mapster;
using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.Domain.Core;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.ClientSubscriptions;

public class UpdateClientSubscriptionInfoRequest : ICommand<Result<ClientSubscriptionSimplifyDto>>
{
    public PreferredMealTime? PreferredDeliveryTime { get; set; } // Preferred meal time for deliveries (e.g., Morning, Evening)
    public DefaultIdType? MealPlanId { get; set; } // Foreign key to the client
    public DefaultIdType? ClientLocationId { get; set; } // Foreign key to the client
    public List<DayOfWeek>? SelectedDeliveryDays { get; set; }
    public bool? HasCutlery { get; set; } // Foreign key to the client
    public bool? Pause { get; set; } // Foreign key to the client
    public bool? IsAutoRenewalEnabled { get; set; } // Foreign key to the client
    public SubscriptionType? SubscriptionType { get; set; } // Foreign key to the client
}


public class UpdateClientSubscriptionInfoRequestValidator : CustomValidator<UpdateClientSubscriptionInfoRequest>
{
    public UpdateClientSubscriptionInfoRequestValidator(IStringLocalizer<UpdateClientSubscriptionInfoRequestValidator> localaizer)
    {
    }
}


public class UpdateClientSubscriptionInfoRequestHandler(
    IRepositoryWithEvents<ClientSubscription> repository,
    IRepository<Client> clientRepo,
    IReadRepository<MealPlanMealType> mealPlanMealTypeRepo,
    ICurrentUser currentUser,
    IPaymobService paymobService,
    IRepository<SubscriptionPlan> subscriptionPlanRepo,
    IClientPortalSettingservice clientPortalSettingservice,
    IStringLocalizer<UpdateClientSubscriptionInfoRequestHandler> localizer
    ) : IRequestHandler<UpdateClientSubscriptionInfoRequest, Result<ClientSubscriptionSimplifyDto>>
{
    private readonly IRepositoryWithEvents<ClientSubscription> _repository = repository;
    private readonly IRepository<Client> _clientRepo = clientRepo;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<ClientSubscriptionSimplifyDto>> Handle(UpdateClientSubscriptionInfoRequest request, CancellationToken cancellationToken)
    {
        bool isClient = _currentUser.GetRoleType()!.Equals(nameof(UserType.Client).ToString(), StringComparison.OrdinalIgnoreCase);
        if (!isClient)
        {
            throw new BadRequestException("only client can register");
        }

        var client = await _clientRepo.FirstOrDefaultAsync(new ExpressionSpecification<Client>(x => x.Id == _currentUser.GetUserId()), cancellationToken);
        _ = client ?? throw new NotFoundException(_localizer["client not found"]);

        var clientSubscription = await _repository.FirstOrDefaultAsync(new ExpressionSpecification<ClientSubscription>(x => x.Id == client.ClientSubscriptionId), cancellationToken);
        _ = clientSubscription ?? throw new NotFoundException(_localizer["client not found"]);

        if (request.MealPlanId.HasValue)
        {
            clientSubscription.MealPlanId = request.MealPlanId.Value;
        }
        if (request.PreferredDeliveryTime.HasValue)
        {
            clientSubscription.PreferredDeliveryTime = request.PreferredDeliveryTime.Value;
        }
        if (request.ClientLocationId.HasValue)
        {
            clientSubscription.ClientLocationId = request.ClientLocationId;
        }

        if (request.IsAutoRenewalEnabled.HasValue)
        {
            clientSubscription.IsAutoRenewalEnabled = request.IsAutoRenewalEnabled.Value;
        }

        if (request.SubscriptionType.HasValue)
        {
            clientSubscription.SubscriptionType = request.SubscriptionType.Value;
        }


        await _repository.UpdateAsync(clientSubscription, withSaveChanges: true, cancellationToken: cancellationToken);

        // _jobService.Enqueue<ISubscriptionsJob>(x => x.CreateSubscriptionDailySelection(clientSubscription.Id));

        return await Result<ClientSubscriptionSimplifyDto>.SuccessAsync(clientSubscription.Adapt<ClientSubscriptionSimplifyDto>());
    }

}

