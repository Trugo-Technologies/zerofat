using Mapster;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.ClientPortal.Application.ClientManagement;
using ZeroFat.ClientPortal.Application.ClientManagement.Clients;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.ClientSubscriptions;
using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.Wizard;

public class GetClientAccountAccessRequest(DefaultIdType clientId) : IQuery<Result<ClientAccountAccessDto>>
{
    public DefaultIdType ClientId { get; set; } = clientId;
}

public class GetClientAccountAccessRequestHandler(
    ICurrentUser currentUser,
    IReadRepository<Client> clientRepo,
    IReadRepository<ClientLocation> locationRepo,
    IReadRepository<ClientSubscription> subscriptionRepo,
    IReadRepository<MealPlan> mealPlanRepo,
    IReadRepository<DailyMealSelection> dailyMealSelectionRepo,
    IStringLocalizer<GetClientAccountAccessRequestHandler> localizer) : IQueryHandler<GetClientAccountAccessRequest, Result<ClientAccountAccessDto>>
{
    public async Task<Result<ClientAccountAccessDto>> Handle(GetClientAccountAccessRequest request, CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);

        var client = await clientRepo.FirstOrDefaultAsync(new ClientByIdSpec<ClientDetailsDto>(request.ClientId), cancellationToken)
            ?? throw new NotFoundException(localizer["Client not found"]);

        var dto = await BuildDtoAsync(
            client,
            locationRepo,
            subscriptionRepo,
            mealPlanRepo,
            dailyMealSelectionRepo,
            cancellationToken);

        return await Result<ClientAccountAccessDto>.SuccessAsync(dto);
    }

    internal static async Task<ClientAccountAccessDto> BuildDtoAsync(
        ClientDetailsDto client,
        IReadRepository<ClientLocation> locationRepo,
        IReadRepository<ClientSubscription> subscriptionRepo,
        IReadRepository<MealPlan> mealPlanRepo,
        IReadRepository<DailyMealSelection> dailyMealSelectionRepo,
        CancellationToken cancellationToken)
    {
        var dto = new ClientAccountAccessDto
        {
            ClientId = client.Id,
            FullName = client.FullName,
            Email = client.Email,
            Mobile = client.Mobile,
            Gender = client.Gender,
            BirthDate = client.BirthDate,
            IsActive = client.IsActive,
            BMI = client.BMI,
            BMR = client.BMR,
            BodyFat = client.BodyFat,
            TDEE = client.TDEE,
            HeightInCM = client.HeightInCM,
            WeightInKG = client.WeightInKG,
            TargetWeightInKG = client.TargetWeightInKG,
            DietitianGoal = client.DietitianGoal,
            TimeToReachGoalInDays = client.TimeToReachGoalInDays,
            NeededCaloriesToReachGoal = client.NeededCaloriesToReachGoal,
            Allergens = client.ClientAllergicIds.Select(_ => _.ToString()).ToList(),
            DeliveryAddresses = await LoadDeliveryAddressesAsync(locationRepo, client.Id, cancellationToken)
        };

        if (client.ClientSubscriptionId.HasValue)
        {
            dto.Subscription = await BuildSummaryAsync(
                client.ClientSubscriptionId.Value,
                client.SubscriptionStatus,
                subscriptionRepo,
                mealPlanRepo,
                dailyMealSelectionRepo,
                cancellationToken);
        }

        return dto;
    }

    internal static async Task<List<ClientAccountAccessAddressDto>> LoadDeliveryAddressesAsync(
        IReadRepository<ClientLocation> locationRepo,
        DefaultIdType clientId,
        CancellationToken cancellationToken)
    {
        var locations = await locationRepo.ListAsync(
            new ExpressionSpecification<ClientLocation>(x => x.ClientId == clientId),
            cancellationToken);

        return locations
            .OrderBy(x => x.CreatedOn)
            .Select(x => new ClientAccountAccessAddressDto
            {
                Id = x.Id,
                Type = x.Type,
                Area = x.Area,
                Building = x.Building,
                Flat = x.Office,
                Street = x.Street
            })
            .ToList();
    }

    internal static async Task<ClientSubscriptionSummaryDto> BuildSummaryAsync(
        DefaultIdType subscriptionId,
        SubscriptionStatus clientStatus,
        IReadRepository<ClientSubscription> subscriptionRepo,
        IReadRepository<MealPlan> mealPlanRepo,
        IReadRepository<DailyMealSelection> dailyMealSelectionRepo,
        CancellationToken cancellationToken)
    {
        var subscription = await subscriptionRepo.FirstOrDefaultAsync(
            new ClientSubscriptionByIdSpec(subscriptionId),
            cancellationToken);

        if (subscription == null)
        {
            return new ClientSubscriptionSummaryDto { SubscriptionStatus = clientStatus };
        }

        var mealPlan = await mealPlanRepo.FirstOrDefaultAsync(
            new ExpressionSpecification<MealPlan>(x => x.Id == subscription.MealPlanId),
            cancellationToken);

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var remainingDays = subscription.EndDate >= today
            ? subscription.EndDate.DayNumber - today.DayNumber
            : 0;

        var deliveredMeals = await dailyMealSelectionRepo.CountAsync(
            new ExpressionSpecification<DailyMealSelection>(x =>
                x.ClientSubscriptionId == subscriptionId && x.Date <= today),
            cancellationToken);

        return new ClientSubscriptionSummaryDto
        {
            SubscriptionId = subscription.Id,
            SubscriptionStatus = subscription.SubscriptionStatus,
            ActiveMealPlanName = mealPlan?.NameEn,
            AverageCalories = subscription.AverageCalories,
            MealsPerDay = subscription.SelectedMealTypes.Sum(x => x.QuantityPerDay),
            StartDate = subscription.StartDate,
            EndDate = subscription.EndDate,
            RemainingDays = Math.Max(remainingDays, 0),
            TotalDeliveredMeals = deliveredMeals
        };
    }
}

public class GetClientSubscriptionSummaryRequest(DefaultIdType clientId) : IQuery<Result<ClientSubscriptionSummaryDto>>
{
    public DefaultIdType ClientId { get; set; } = clientId;
}

public class GetClientSubscriptionSummaryRequestHandler(
    ICurrentUser currentUser,
    IReadRepository<Client> clientRepo,
    IReadRepository<ClientSubscription> subscriptionRepo,
    IReadRepository<MealPlan> mealPlanRepo,
    IReadRepository<DailyMealSelection> dailyMealSelectionRepo,
    IStringLocalizer<GetClientSubscriptionSummaryRequestHandler> localizer) : IQueryHandler<GetClientSubscriptionSummaryRequest, Result<ClientSubscriptionSummaryDto>>
{
    public async Task<Result<ClientSubscriptionSummaryDto>> Handle(GetClientSubscriptionSummaryRequest request, CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);
        var client = await clientRepo.FirstOrDefaultAsync(new ExpressionSpecification<Client>(x => x.Id == request.ClientId), cancellationToken)
            ?? throw new NotFoundException(localizer["Client not found"]);

        if (!client.ClientSubscriptionId.HasValue)
        {
            return await Result<ClientSubscriptionSummaryDto>.SuccessAsync(new ClientSubscriptionSummaryDto
            {
                SubscriptionStatus = client.SubscriptionStatus
            });
        }

        var summary = await GetClientAccountAccessRequestHandler.BuildSummaryAsync(
            client.ClientSubscriptionId.Value,
            client.SubscriptionStatus,
            subscriptionRepo,
            mealPlanRepo,
            dailyMealSelectionRepo,
            cancellationToken);

        return await Result<ClientSubscriptionSummaryDto>.SuccessAsync(summary);
    }
}

public class UpdateClientAccountAccessRequest : ICommand<Result<ClientAccountAccessDto>>
{
    public DefaultIdType ClientId { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Mobile { get; set; }
    public Gender? Gender { get; set; }
    public DateTime? BirthDate { get; set; }
    public List<ClientAccountAccessAddressDto> DeliveryAddresses { get; set; } = [];
}

public class UpdateClientAccountAccessRequestValidator : CustomValidator<UpdateClientAccountAccessRequest>
{
    public UpdateClientAccountAccessRequestValidator(IStringLocalizer<UpdateClientAccountAccessRequestValidator> localizer)
    {
        RuleFor(x => x.ClientId).NotEmpty();

        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage(localizer["Full name is required."]);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage(localizer["A valid email is required."]);

        RuleFor(x => x.Mobile)
            .NotEmpty()
            .WithMessage(localizer["Phone number is required."]);

        RuleFor(x => x.Gender)
            .IsInEnum()
            .When(x => x.Gender.HasValue)
            .WithMessage(localizer["Gender must be a valid value."]);

        RuleForEach(x => x.DeliveryAddresses).ChildRules(address =>
        {
            address.RuleFor(a => a.Type)
                .NotEmpty()
                .WithMessage(localizer["Location type is required."]);
        });
    }
}

public class UpdateClientAccountAccessRequestHandler(
    ICurrentUser currentUser,
    IRepository<Client> clientRepo,
    IReadRepository<Client> clientReadRepo,
    IRepository<ClientLocation> locationRepo,
    IReadRepository<ClientLocation> locationReadRepo,
    IReadRepository<ClientSubscription> subscriptionRepo,
    IReadRepository<MealPlan> mealPlanRepo,
    IReadRepository<DailyMealSelection> dailyMealSelectionRepo,
    IRepository<ClientAccountActivityLog> activityLogRepo,
    IStripeService stripeService,
    IStringLocalizer<UpdateClientAccountAccessRequestHandler> localizer) : ICommandHandler<UpdateClientAccountAccessRequest, Result<ClientAccountAccessDto>>
{
    public async Task<Result<ClientAccountAccessDto>> Handle(UpdateClientAccountAccessRequest request, CancellationToken cancellationToken)
    {
        SubscriptionWizardAdminHelper.EnsureAdmin(currentUser, localizer);

        var client = await clientRepo.GetByIdAsync(request.ClientId, cancellationToken)
            ?? throw new NotFoundException(localizer["Client not found"]);

        client.FullName = request.FullName;
        client.Email = request.Email;
        client.Mobile = request.Mobile;
        client.Gender = request.Gender;
        client.BirthDate = request.BirthDate;

        await UpsertDeliveryAddressesAsync(request.ClientId, request.DeliveryAddresses, cancellationToken);

        if (client.StripeId.HasValue())
        {
            await stripeService.UpdateCustomerOnStripe(
                client.StripeId!,
                client.Email,
                client.FullName,
                client.Mobile,
                client.Id.ToString());
        }

        await clientRepo.UpdateAsync(client, cancellationToken);

        await ClientAccountActivityLogHelper.LogAsync(
            activityLogRepo, currentUser, request.ClientId,
            ClientAccountActivityAction.ProfileUpdated,
            null,
            request.FullName,
            cancellationToken: cancellationToken);

        var updatedClient = await clientReadRepo.FirstOrDefaultAsync(
            new ClientByIdSpec<ClientDetailsDto>(request.ClientId),
            cancellationToken)
            ?? throw new NotFoundException(localizer["Client not found"]);

        var dto = await GetClientAccountAccessRequestHandler.BuildDtoAsync(
            updatedClient,
            locationReadRepo,
            subscriptionRepo,
            mealPlanRepo,
            dailyMealSelectionRepo,
            cancellationToken);

        return await Result<ClientAccountAccessDto>.SuccessAsync(dto);
    }

    private async Task UpsertDeliveryAddressesAsync(
        DefaultIdType clientId,
        List<ClientAccountAccessAddressDto> addresses,
        CancellationToken cancellationToken)
    {
        foreach (var address in addresses)
        {
            if (address.Id.HasValue)
            {
                var location = await locationRepo.GetByIdAsync(address.Id.Value, cancellationToken)
                    ?? throw new NotFoundException(localizer["Location not found"]);

                if (location.ClientId != clientId)
                {
                    throw new ForbiddenException(localizer["Location does not belong to this client."]);
                }

                ApplyAddressFields(location, address);
                await locationRepo.UpdateAsync(location, cancellationToken);
                continue;
            }

            var newLocation = new ClientLocation { ClientId = clientId };
            ApplyAddressFields(newLocation, address);
            await locationRepo.AddAsync(newLocation, cancellationToken);
        }
    }

    private static void ApplyAddressFields(ClientLocation location, ClientAccountAccessAddressDto address)
    {
        location.Type = address.Type;
        location.Area = address.Area;
        location.Building = address.Building;
        location.Office = address.Flat;
        location.Street = address.Street;
    }
}
