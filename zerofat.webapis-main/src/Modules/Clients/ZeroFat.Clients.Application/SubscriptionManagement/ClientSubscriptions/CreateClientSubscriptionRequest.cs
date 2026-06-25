using System.Text.Json;
using System.Text.Json.Serialization;
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
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.ClientSubscriptions;

public class CreateClientSubscriptionRequest : ICommand<Result<ClientSubscriptionSimplifyDto>>
{
    public CreateClientSubscriptionRequest()
    {
        MealTypeSelections = [];
    }
    public SubscriptionType SubscriptionType { get; set; }
    public PreferredMealTime PreferredDeliveryTime { get; set; } // Preferred meal time for deliveries (e.g., Morning, Evening)
    public DefaultIdType MealPlanId { get; set; } // Foreign key to the client
    public DateOnly StartDate { get; set; } // Foreign key to the client
    public bool IsAutoRenewalEnabled { get; set; } // Foreign key to the client
    public DefaultIdType? ClientLocationId { get; set; } // Foreign key to the client
    public List<DayOfWeek> SelectedDeliveryDays { get; set; } = [];
    public string? CouponCode { get; set; }
    public List<CreateClientSubscriptionMealTypeSelection> MealTypeSelections { get; set; }

    [JsonIgnore]
    public DateOnly? MinimumStartDate { get; set; }
    [JsonIgnore]
    public int? OffsetSubscriptionInDays { get; set; }
}

public class CreateClientSubscriptionMealTypeSelection
{
    public int QuantityPerDay { get; set; } // The number of meals of this type the client has selected (e.g., 3 breakfasts)
    public string? Name { get; set; }
    public DefaultIdType MealTypeId { get; set; } // Foreign key to the client
}


public class CreateClientSubscriptionRequestValidator : CustomValidator<CreateClientSubscriptionRequest>
{
    public CreateClientSubscriptionRequestValidator(
        ICurrentUser currentUser,
        IStringLocalizer<CreateClientSubscriptionRequestValidator> localizer,
        IClientPortalSettingservice clientPortalSettingservice)
    {
        RuleFor(x => x)
          .Must(x => currentUser.GetRoleType()!.Equals(nameof(UserType.Client).ToString(), StringComparison.OrdinalIgnoreCase))
          .WithMessage(localizer["Only clients can register."]);

        // Validate SubscriptionType
        RuleFor(x => x.SubscriptionType)
            .IsInEnum()
            .WithMessage(localizer["Invalid subscription type."]);

        // Validate PreferredMealTime
        RuleFor(x => x.PreferredDeliveryTime)
            .IsInEnum()
            .WithMessage(localizer["Invalid preferred meal time."]);

        // Validate DaysPerWeek
        RuleFor(x => x.SelectedDeliveryDays)
            .NotEmpty()
                .WithMessage(localizer["At least one day per week must be selected."])
            .Must(days => days.Count >= 5) // At least 5 days must be selected
                .WithMessage(localizer["At least 5 days per week must be selected."])
            .Must(days => days.TrueForAll(day => Enum.IsDefined(day)))
                .WithMessage(localizer["Invalid day of week selected."]);

        // Validate StartDate
        RuleFor(x => x.StartDate)
            .MustAsync(async (requst, startDate, cancellationToken) =>
            {
                var offsetSubscriptionInDays = await clientPortalSettingservice.GetOffsetSubscriptionInDays();
                var minimumStartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(offsetSubscriptionInDays));

                requst.MinimumStartDate = minimumStartDate;
                requst.OffsetSubscriptionInDays = offsetSubscriptionInDays;
                return startDate >= minimumStartDate;
            })
            .WithMessage(request =>
            {
                // Ensure MinimumStartDate is set before constructing the message
                var minimumStartDate = request.MinimumStartDate?.ToString("yyyy-MM-dd");
                return localizer["Start date must be equal to or greater than the minimum allowed date ({0}).", minimumStartDate];
            });
    }
}


public class CreateClientSubscriptionRequestHandler(
    IRepositoryWithEvents<ClientSubscription> repository,
    IRepository<Client> clientRepo,
    IReadRepository<MealPlanMealType> mealPlanMealTypeRepo,
    ICurrentUser currentUser,
    IStripeService stripeService,
    IRepository<MealPlan> mealPlanRepo,
    IClientPortalSettingservice clientPortalSettingservice,
    ISubscriptionPricingService pricingService,
    IStringLocalizer<CreateClientSubscriptionRequestHandler> localizer
    ) : IRequestHandler<CreateClientSubscriptionRequest, Result<ClientSubscriptionSimplifyDto>>
{
    private readonly IRepositoryWithEvents<ClientSubscription> _repository = repository;
    private readonly IRepository<MealPlan> _mealPlanRepo = mealPlanRepo;
    private readonly IRepository<Client> _clientRepo = clientRepo;
    private readonly IReadRepository<MealPlanMealType> _mealPlanMealTypeRepo = mealPlanMealTypeRepo;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IStripeService _stripeService = stripeService;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<ClientSubscriptionSimplifyDto>> Handle(CreateClientSubscriptionRequest request, CancellationToken cancellationToken)
    {
        var client = await _clientRepo.FirstOrDefaultAsync(new ExpressionSpecification<Client>(x => x.Id == _currentUser.GetUserId()), cancellationToken);
        _ = client ?? throw new NotFoundException(_localizer["client not found"]);

        var mealPlan = await _mealPlanRepo.FirstOrDefaultAsync(new ExpressionSpecification<MealPlan>(x => x.Id == request.MealPlanId), cancellationToken);
        _ = mealPlan ?? throw new NotFoundException(_localizer["Invalid Meal Plan."]);

        if (client.ClientSubscriptionId != null && client.SubscriptionStatus == SubscriptionStatus.Active)
        {
            throw new ForbiddenException(_localizer["client already have subscription"]);
        }

        ClientSubscription clientSubscription = new()
        {
            StartDate = request.StartDate,
            SelectedDeliveryDays = request.SelectedDeliveryDays,
            IsAutoRenewalEnabled = request.IsAutoRenewalEnabled,
            MealPlanId = request.MealPlanId,
            PreferredDeliveryTime = request.PreferredDeliveryTime,
            ClientId = client.Id,
            PaymentStatus = PaymentStatus.Pending,
            SubscriptionStatus = SubscriptionStatus.Pending,
            SubscriptionType = request.SubscriptionType,
            ClientLocationId = request.ClientLocationId,
        };

        var mealTypePricingInputs = new List<SubscriptionMealTypePricingInput>();

        foreach (var mealTypeSelection in request.MealTypeSelections.Where(x => x.QuantityPerDay > 0))
        {
            var mealPlanMealType = await _mealPlanMealTypeRepo.FirstOrDefaultAsync(new ExpressionSpecificationProjecting<MealPlanMealType, MealPlanMealTypeDto>(x => x.MealPlanId == request.MealPlanId && x.MealTypeId == mealTypeSelection.MealTypeId), cancellationToken);
            if (mealPlanMealType == null)
            {
                throw new BadRequestException(_localizer["error in subscription please contact our admin through chat."]);
            }

            var price = mealPlanMealType.Price.GetValueOrDefault();
            var averageCalories = mealPlanMealType.AverageCalories.GetValueOrDefault();

            clientSubscription.SelectedMealTypes.Add(new MealTypeSelection()
            {
                QuantityPerDay = mealTypeSelection.QuantityPerDay,
                MealTypeId = mealTypeSelection.MealTypeId,
                Price = price,
                MealTypeNameAr = mealPlanMealType.MealType?.NameAr,
                MealTypeNameEn = mealPlanMealType.MealType?.NameEn,
            });

            mealTypePricingInputs.Add(new SubscriptionMealTypePricingInput
            {
                MealTypeId = mealTypeSelection.MealTypeId,
                QuantityPerDay = mealTypeSelection.QuantityPerDay,
                Price = price,
                AverageCalories = averageCalories,
                MealTypeNameEn = mealPlanMealType.MealType?.NameEn,
                MealTypeNameAr = mealPlanMealType.MealType?.NameAr
            });
        }

        // Pricing via shared ISubscriptionPricingService (same logic as admin wizard preview/finalize)
        var pricing = await pricingService.CalculateAsync(new SubscriptionPricingInput
        {
            SubscriptionType = request.SubscriptionType,
            SelectedDeliveryDays = request.SelectedDeliveryDays,
            StartDate = request.StartDate,
            PromoCode = request.CouponCode,
            MealTypeSelections = mealTypePricingInputs
        }, cancellationToken);

        clientSubscription.EndDate = pricing.EndDate;
        clientSubscription.TotalCost = pricing.TotalCost;
        clientSubscription.AverageCalories = pricing.AverageCalories;
        clientSubscription.VatAmount = pricing.VatAmount;
        clientSubscription.PromoCode = request.CouponCode;

        var mealSelections = clientSubscription.SelectedMealTypes
            .Select(x => new
            {
                x.MealTypeId,
                MealTypeName = x.MealTypeNameEn,
                x.Price,
                x.QuantityPerDay
            })
            .ToList();

        var productName = ConstructProductDescription(clientSubscription.SelectedMealTypes, request.SelectedDeliveryDays);
        var serializedMealSelections = JsonSerializer.Serialize(mealSelections);
        var (id, url) = await _stripeService.CreateSubscriptionLink(
            clientSubscription.Id.ToString(),
            customerId: client.StripeId!,
            productId: mealPlan.StripeId ?? string.Empty,
            amount: clientSubscription.TotalCost,
            mealSelections: serializedMealSelections,
            productName: mealPlan.NameEn!,
            description: productName,
            mealPlanId: mealPlan.Id,
            days: request.SelectedDeliveryDays,
            isRecurring: false, // request.IsAutoRenewalEnabled
            startDate: request.StartDate,
            endDate: clientSubscription.EndDate,
            offsetSubscription: request.OffsetSubscriptionInDays,
            subscriptionType: request.SubscriptionType,
            couponCode: request.CouponCode);

        clientSubscription.PaymentQuickLink = url;
        clientSubscription.PaymentOrderId = id;

        clientSubscription.NextRenewalDate = clientSubscription.EndDate.AddDays(1 - request.OffsetSubscriptionInDays.GetValueOrDefault());

        await _repository.AddAsync(clientSubscription, withSaveChanges: true, cancellationToken: cancellationToken);
        client.ClientSubscriptionId = clientSubscription.Id;
        client.SubscriptionStatus = clientSubscription.SubscriptionStatus;

        await _clientRepo.SaveChangesAsync(cancellationToken);

        // _jobService.Enqueue<ISubscriptionsJob>(x => x.CreateSubscriptionDailySelection(clientSubscription.Id));

        return await Result<ClientSubscriptionSimplifyDto>.SuccessAsync(clientSubscription.Adapt<ClientSubscriptionSimplifyDto>());
    }


    private static string ConstructProductDescription(ICollection<MealTypeSelection> selections, List<DayOfWeek> days)
    {
        var productName = string.Join(", ", selections.Select(s => $"{s.QuantityPerDay} {s.MealTypeNameEn}"));
        if (days.Count == 7)
        {
            productName += " (every day)";
        }
        else
        {
            productName += $" (every {days.Count} days)";
        }

        return productName;
    }

}

