using Ardalis.Specification;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.MealPlanning;
using ZeroFat.NutriPlan.Domain.MenuPlanning;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.DailyMealSelections;
public class UpdateDailyMealSelectionRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType? Id { get; set; }
    public DefaultIdType? MealId { get; set; }
    public DefaultIdType? CustomMealId { get; set; }
    public DefaultIdType? DailyMenuMealId { get; set; }
    public string? PaymentMethodId { get; set; }
}

public class UpdateDailyMealSelectionRequestValidator : AbstractValidator<UpdateDailyMealSelectionRequest>
{
    public UpdateDailyMealSelectionRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Please select a meal to update");

        RuleFor(x => x.MealId)
            .NotEmpty().WithMessage("Please choose a new meal")
            .When(x => x.CustomMealId == null);

        RuleFor(x => x.DailyMenuMealId)
            .NotEmpty().WithMessage("Please select a menu option")
            .When(x => x.CustomMealId == null);
    }
}


public class UpdateDailyMealSelectionRequestHandler : IRequestHandler<UpdateDailyMealSelectionRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<DailyMealSelection> _repository;
    private readonly IRepositoryWithEvents<Meal> _mealRepo;
    private readonly IRepositoryWithEvents<CustomMeal> _customizeMealRepo;
    private readonly IRepositoryWithEvents<DailyMenuMeal> _dailyMenuMealRepo;
    private readonly IRepositoryWithEvents<DailySelection> _dailySelectionRepo;
    private readonly IRepositoryWithEvents<Payment> _paymentRepo;
    private readonly ILogger<UpdateDailyMealSelectionRequestHandler> _logger;
    private readonly IStripeService _stripeService;
    private readonly IStringLocalizer<UpdateDailyMealSelectionRequestHandler> _localizer;
    private readonly IClientPortalSettingservice _clientPortalSettingsService;
    private readonly IRepositoryWithEvents<ClientLoyaltyPoint> _clientLoyaltyPointRepo;
    private readonly IRepositoryWithEvents<Client> _clientRepo;

    public UpdateDailyMealSelectionRequestHandler(
        IRepositoryWithEvents<DailyMealSelection> repository,
        IRepositoryWithEvents<Meal> mealRepo,
        IRepositoryWithEvents<Client> clientRepo,
        IRepositoryWithEvents<CustomMeal> customizeMealRepo,
        IRepositoryWithEvents<DailyMenuMeal> dailyMenuMealRepo,
        IRepositoryWithEvents<DailySelection> dailySelectionRepo,
        IRepositoryWithEvents<Payment> paymentRepo,
        ILogger<UpdateDailyMealSelectionRequestHandler> logger,
        IStripeService stripeService,
        IStringLocalizer<UpdateDailyMealSelectionRequestHandler> localizer,
        IClientPortalSettingservice clientPortalSettingsService,
        IRepositoryWithEvents<ClientLoyaltyPoint> clientLoyaltyPointRepo)
    {
        _repository = repository;
        _mealRepo = mealRepo;
        _dailyMenuMealRepo = dailyMenuMealRepo;
        _dailySelectionRepo = dailySelectionRepo;
        _paymentRepo = paymentRepo;
        _logger = logger;
        _stripeService = stripeService;
        _localizer = localizer;
        _clientPortalSettingsService = clientPortalSettingsService;
        _clientLoyaltyPointRepo = clientLoyaltyPointRepo;
        _clientRepo = clientRepo;
        _customizeMealRepo = customizeMealRepo;
    }

    public async Task<Result<DefaultIdType>> Handle(UpdateDailyMealSelectionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(request.Id!.Value, cancellationToken);
            _ = entity ?? throw new NotFoundException("We couldn't find your selected meal. Please try again.");


            // Check if the selection is actually changing
            if ((entity.MealId == request.MealId && request.CustomMealId == null) ||
                (entity.CustomMealId == request.CustomMealId && request.CustomMealId != null))
            {
                throw new BadRequestException("You've selected the same meal. Please choose a different one.");
            }


            var dailySelection = await _dailySelectionRepo.GetByIdAsync(entity.DailySelectionId, cancellationToken);
            _ = dailySelection ?? throw new NotFoundException("Your daily menu couldn't be found. Please refresh and try again.");

            if (request.CustomMealId != null)
            {
                await HandleCustomMealSelection(request, entity, dailySelection, cancellationToken);
            }
            else
            {
                await HandleStandardMealSelection(request, entity, dailySelection, cancellationToken);
            }

            await _repository.SaveChangesAsync(cancellationToken);

            return await Result<DefaultIdType>.SuccessAsync(entity.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating daily meal selection {DailyMealSelectionId}", request.Id);
            return await Result<DefaultIdType>.FailAsync(ex.Message);
        }
    }

    private async Task HandleCustomMealSelection(
        UpdateDailyMealSelectionRequest request,
        DailyMealSelection entity,
        DailySelection dailySelection,
        CancellationToken cancellationToken)
    {
        var customMeal = await _customizeMealRepo.GetByIdAsync(request.CustomMealId!.Value, cancellationToken)
            ?? throw new NotFoundException("The custom meal you selected isn't available. Please choose another.");

        // Calculate price difference
        var priceDifference = customMeal.TotalPrice - entity.BasePrice;
        bool requiresPayment = priceDifference > 0;

        // Check if payment is needed for the upgrade
        if (requiresPayment)
        {
            if (string.IsNullOrEmpty(request.PaymentMethodId))
            {
                _logger.LogWarning("No payment method provided for custom meal upgrade {DailyMealSelectionId}", entity.Id);
                throw new BadRequestException("To select this custom meal, please add a payment method.");
            }

            var client = await _clientRepo.GetByIdAsync(entity.ClientId, cancellationToken);
            await ProcessPaymentForCustomMeal(entity, client, priceDifference, request.PaymentMethodId, customMeal, cancellationToken);
        }


        // Subtract old nutritional values
        SubtractOldNutritionalValues(dailySelection, entity);

        // Update entity with custom meal details
        entity.CustomMealId = request.CustomMealId;
        entity.MealId = customMeal.BaseMealId;
        entity.MealSelectionType = MealSelectionType.Custom;
        entity.AdjustedPrice = customMeal.TotalPrice;
        entity.PriceAdjustmentReason = requiresPayment
                ? $"Custom meal selected (+{priceDifference})"
                : "Custom meal selected";

        // Add new nutritional values
        AddNewNutritionalValues(dailySelection, customMeal.TotalCalories, customMeal.TotalProtein, customMeal.TotalFat, customMeal.TotalCarbs);

        // Update entity's nutritional info
        UpdateMealNutritionalInfo(entity, customMeal.TotalCalories, customMeal.TotalProtein, customMeal.TotalFat, customMeal.TotalCarbs);
    }

    private async Task HandleStandardMealSelection(
    UpdateDailyMealSelectionRequest request,
    DailyMealSelection entity,
    DailySelection dailySelection,
    CancellationToken cancellationToken)
    {
        var dailyMenuMeal = await _dailyMenuMealRepo.FirstOrDefaultAsync(
            new DailyMenuMealByIdSpec(request.DailyMenuMealId!.Value), cancellationToken)
            ?? throw new NotFoundException("The meal option you selected isn't available. Please choose another.");

        // Check for price difference if upgrading
        if (dailyMenuMeal.DailyMenu!.Price > entity.BasePrice && dailyMenuMeal.DailyMenu.Price > entity.AdjustedPrice.GetValueOrDefault())
        {
            var cost = dailyMenuMeal.DailyMenu.Price.GetValueOrDefault() - entity.BasePrice;
            entity.AdjustedPrice = dailyMenuMeal.DailyMenu.Price.GetValueOrDefault();
            entity.PriceAdjustmentReason = $"Your meal was upgraded (+{cost}) to {dailyMenuMeal.Meal!.NameEn}";

            if (string.IsNullOrEmpty(request.PaymentMethodId))
            {
                _logger.LogWarning("No payment method provided for daily meal selection {DailyMealSelectionId}", entity.Id);
                throw new BadRequestException("To upgrade your meal, please add a payment method.");
            }

            var client = await _clientRepo.GetByIdAsync(entity.ClientId, cancellationToken);
            await ProcessPaymentAsync(entity, client, cost, request.PaymentMethodId, cancellationToken);
        }

        // Subtract old nutritional values
        SubtractOldNutritionalValues(dailySelection, entity);

        // Update entity with new meal details
        entity.MealId = request.MealId;
        entity.CustomMealId = null;
        entity.CustomeMealName = null;
        entity.MealSelectionType = MealSelectionType.Default;
        entity.MealPlanId = dailyMenuMeal.DailyMenu.MealPlanId;

        // Add new nutritional values
        AddNewNutritionalValues(dailySelection, dailyMenuMeal.Meal.Calories, dailyMenuMeal.Meal.Protein, dailyMenuMeal.Meal.Fat, dailyMenuMeal.Meal.Carbs);

        // Update entity's nutritional info
        UpdateMealNutritionalInfo(entity, dailyMenuMeal.Meal.Calories, dailyMenuMeal.Meal.Protein, dailyMenuMeal.Meal.Fat, dailyMenuMeal.Meal.Carbs);
    }

    private void AddNewNutritionalValues(DailySelection dailySelection, double calories, double protein, double fat, double carbs)
    {
        dailySelection.TotalCarbohydrates += carbs;
        dailySelection.TotalCalories += calories;
        dailySelection.TotalFats += fat;
        dailySelection.TotalProteins += protein;
    }

    private void UpdateMealNutritionalInfo(DailyMealSelection entity, double calories, double protein, double fat, double carbs)
    {
        entity.TotalCalories = calories;
        entity.TotalProteins = protein;
        entity.TotalFats = fat;
        entity.TotalCarbohydrates = carbs;
    }

    private void SubtractOldNutritionalValues(DailySelection dailySelection, DailyMealSelection entity)
    {
        dailySelection.TotalCarbohydrates -= entity.TotalCarbohydrates;
        dailySelection.TotalCalories -= entity.TotalCalories;
        dailySelection.TotalFats -= entity.TotalFats;
        dailySelection.TotalProteins -= entity.TotalProteins;
    }

    private async Task<PaymentIntentResponseDTO> ProcessPaymentAsync(
        DailyMealSelection dailyMealSelection,
        Client? client,
        decimal cost,
        string paymentMethodId,
        CancellationToken cancellationToken)
    {
        var paymentIntent = await _stripeService.CreateAddOnPaymentLink(
            dailyMealSelection.Id.ToString(),
            paymentMethodId,
            client!.StripeId!,
            cost,
            dailyMealSelection.PriceAdjustmentReason!);

        if (paymentIntent.Status != "succeeded")
        {
            _logger.LogWarning("Payment failed for daily meal selection {DailyMealSelectionId} with status {Status}",
                dailyMealSelection.Id.ToString(), paymentIntent.Status);
            throw new BadRequestException(_localizer["Payment processing failed"]);
        }

        var payment = CreatePaymentRecord(client, paymentIntent, dailyMealSelection.PriceAdjustmentReason);
        await _paymentRepo.AddAsync(payment, withSaveChanges: false, cancellationToken: cancellationToken);
        _logger.LogInformation("Created payment record {PaymentId}", payment.Id);

        await AddLoyaltyPoints(client, payment, dailyMealSelection.PriceAdjustmentReason, cancellationToken);
        dailyMealSelection.IsPaid = true;

        return paymentIntent;
    }

    private async Task AddLoyaltyPoints(
        Client client,
        Payment payment,
        string? mealDescription,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Calculating loyalty points for payment {PaymentId}", payment.Id);

        var points = await _clientPortalSettingsService.GetOnePointEqualInMoney();
        var pointsEarned = points == 0 ? 0 : (int)(payment.Amount / points);

        await _clientLoyaltyPointRepo.AddAsync(new ClientLoyaltyPoint
        {
            ClientId = client.Id,
            Reason = mealDescription,
            Source = "Payment",
            TransactionId = payment.Id.ToString(),
            Points = pointsEarned,
            Type = TransactionType.Earn,
            DateEarned = DateTime.UtcNow,
        }, withSaveChanges: false, cancellationToken: cancellationToken);

        _logger.LogInformation("Awarded {PointsEarned} loyalty points to client {ClientId}",
            pointsEarned, client.Id);
    }

    private Payment CreatePaymentRecord(
        Client client,
        PaymentIntentResponseDTO paymentIntent,
        string? mealDescription)
    {
        _logger.LogDebug("Creating payment record for payment intent {PaymentIntentId}", paymentIntent.Id);

        return new Payment
        {
            ClientId = client.Id,
            ClientSubscriptionId = client.ClientSubscriptionId!.Value,
            InvoiceNumber = paymentIntent.Id,
            TransactionId = paymentIntent.LatestChargeId ?? paymentIntent.Id,
            PaymentStatus = PaymentStatus.Paid,
            PaymentMethod = paymentIntent.PaymentMethodType ?? "unknown",
            Amount = paymentIntent.Amount / 100m,
            PaymentGateway = "Stripe",
            Reason = mealDescription,
            Currency = paymentIntent.Currency,
            PaymentDate = DateTime.UtcNow,
            OrderId = paymentIntent.OrderId
        };
    }

    private async Task ProcessPaymentForCustomMeal(
    DailyMealSelection dailyMealSelection,
    Client client,
    decimal amount,
    string paymentMethodId,
    CustomMeal customMeal,
    CancellationToken cancellationToken)
    {
        var description = $"Custom meal upgrade to {customMeal.NameEn}";

        var paymentIntent = await _stripeService.CreateAddOnPaymentLink(
            dailyMealSelection.Id.ToString(),
            paymentMethodId,
            client.StripeId!,
            amount,
            description);

        if (paymentIntent.Status != "succeeded")
        {
            _logger.LogWarning("Payment failed for custom meal selection {DailyMealSelectionId} with status {Status}",
                dailyMealSelection.Id.ToString(), paymentIntent.Status);
            throw new BadRequestException(_localizer["Payment processing failed for custom meal"]);
        }

        var payment = new Payment
        {
            ClientId = client.Id,
            ClientSubscriptionId = client.ClientSubscriptionId!.Value,
            InvoiceNumber = paymentIntent.Id,
            TransactionId = paymentIntent.LatestChargeId ?? paymentIntent.Id,
            PaymentStatus = PaymentStatus.Paid,
            PaymentMethod = paymentIntent.PaymentMethodType ?? "unknown",
            Amount = paymentIntent.Amount / 100m,
            PaymentGateway = "Stripe",
            Reason = description,
            Currency = paymentIntent.Currency,
            PaymentDate = DateTime.UtcNow,
            OrderId = paymentIntent.OrderId
        };

        await _paymentRepo.AddAsync(payment, withSaveChanges: false, cancellationToken: cancellationToken);
        _logger.LogInformation("Created payment record {PaymentId} for custom meal", payment.Id);

        await AddLoyaltyPoints(client, payment, description, cancellationToken);
        dailyMealSelection.IsPaid = true;
    }
}

public class DailyMenuMealByIdSpec : Specification<DailyMenuMeal>
{
    public DailyMenuMealByIdSpec(DefaultIdType id)
    {
        Query.Where(p => p.Id == id)
            .Include(x => x.DailyMenu)
            .Include(x => x.Meal);
    }
}
