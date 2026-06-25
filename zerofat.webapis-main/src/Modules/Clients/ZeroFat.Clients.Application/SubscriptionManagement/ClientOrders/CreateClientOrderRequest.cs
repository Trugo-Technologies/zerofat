using Mapster;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.Domain.Common;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.ClientOrders;

public class CreateClientOrderRequest : ICommand<Result<ClientOrderDto>>
{
    public DateOnly Date { get; set; }
    public bool IsRecurring { get; set; }
    public string? PaymentMethodId { get; set; }
    public List<CreateClientOrderItem> Meals { get; set; } = [];
}

public class CreateClientOrderItem
{
    public int QuantityPerDay { get; set; }
    public DefaultIdType MealId { get; set; }
}

public class CreateClientOrderRequestValidator : CustomValidator<CreateClientOrderRequest>
{
    public CreateClientOrderRequestValidator(IStringLocalizer<CreateClientOrderRequestValidator> localizer)
    {
        // Add validation rules here if needed
    }
}

public class CreateClientOrderRequestHandler : IRequestHandler<CreateClientOrderRequest, Result<ClientOrderDto>>
{
    private readonly IRepositoryWithEvents<ClientSubscription> _clientSubscriptionRepo;
    private readonly IRepositoryWithEvents<Payment> _paymentRepo;
    private readonly IRepository<Client> _clientRepo;
    private readonly IRepository<ClientOrder> _clientOrderRepo;
    private readonly IRepository<Meal> _mealRepo;
    private readonly ICurrentUser _currentUser;
    private readonly IStripeService _stripeService;
    private readonly IStringLocalizer<CreateClientOrderRequestHandler> _localizer;
    private readonly IClientPortalSettingservice _clientPortalSettingservice;
    private readonly IJobService _jobService;
    private readonly ILogger<CreateClientOrderRequestHandler> _logger;
    private readonly IRepositoryWithEvents<ClientLoyaltyPoint> _clientLoyaltyPointRepo;

    public CreateClientOrderRequestHandler(
        IRepositoryWithEvents<ClientSubscription> clientSubscriptionRepo,
        IRepositoryWithEvents<Payment> paymentRepo,
        IRepository<Client> clientRepo,
        IRepository<ClientOrder> clientOrderRepo,
        IRepository<Meal> mealRepo,
        ICurrentUser currentUser,
        IStripeService stripeService,
        IJobService jobService,
        IStringLocalizer<CreateClientOrderRequestHandler> localizer,
        IClientPortalSettingservice clientPortalSettingservice,
        ILogger<CreateClientOrderRequestHandler> logger,
        IRepositoryWithEvents<ClientLoyaltyPoint> clientLoyaltyPointRepo)
    {
        _clientSubscriptionRepo = clientSubscriptionRepo;
        _paymentRepo = paymentRepo;
        _clientRepo = clientRepo;
        _clientOrderRepo = clientOrderRepo;
        _mealRepo = mealRepo;
        _currentUser = currentUser;
        _stripeService = stripeService;
        _jobService = jobService;
        _localizer = localizer;
        _clientPortalSettingservice = clientPortalSettingservice;
        _logger = logger;
        _clientLoyaltyPointRepo = clientLoyaltyPointRepo;
    }

    public async Task<Result<ClientOrderDto>> Handle(CreateClientOrderRequest request, CancellationToken cancellationToken)
    {
        using var scope = _logger.BeginScope("Creating client order for user {UserId}", _currentUser.GetUserId());

        try
        {
            // 1. Authorization Check
            ValidateUserIsClient();

            // 2. Fetch Client Data
            var client = await GetClientAsync(cancellationToken);
            var clientSubscription = await GetClientSubscriptionAsync(client, cancellationToken);

            // 3. Validate Subscription Status
            ValidateSubscriptionStatus(client);

            // 4. Create Order Entity
            var order = await CreateOrderAsync(request, client, clientSubscription, cancellationToken);

            // 5. Process Payment
            var paymentResult = await ProcessPaymentAsync(request, order, client, clientSubscription, cancellationToken);

            // 6. Save Changes
            await _clientOrderRepo.AddAsync(order, cancellationToken);
            _logger.LogInformation("Successfully created order {OrderId}", order.Id);


            _jobService.Enqueue<ISubscriptionMealOrchestrator>(x => x.CreateDailyMealSelectionsForOrderAsync(order.Id));

            return await Result<ClientOrderDto>.SuccessAsync(order.Adapt<ClientOrderDto>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create client order");
            throw; // Let the global exception handler process it
        }
    }

    private void ValidateUserIsClient()
    {
        bool isClient = _currentUser.GetRoleType()!.Equals(nameof(UserType.Client), StringComparison.OrdinalIgnoreCase);
        if (!isClient)
        {
            _logger.LogWarning("Non-client user attempted to create order");
            throw new BadRequestException(_localizer["Only clients can create orders"]);
        }
    }

    private async Task<Client> GetClientAsync(CancellationToken cancellationToken)
    {
        var client = await _clientRepo.FirstOrDefaultAsync(
            new ExpressionSpecification<Client>(x => x.Id == _currentUser.GetUserId()),
            cancellationToken);

        if (client == null)
        {
            _logger.LogWarning("Client not found for user {UserId}", _currentUser.GetUserId());
            throw new NotFoundException(_localizer["Client not found"]);
        }

        return client;
    }

    private async Task<ClientSubscription> GetClientSubscriptionAsync(Client client, CancellationToken cancellationToken)
    {
        if (client.ClientSubscriptionId == null)
        {
            _logger.LogWarning("Client {ClientId} has no subscription", client.Id);
            throw new NotFoundException(_localizer["No subscription found"]);
        }

        var subscription = await _clientSubscriptionRepo.FirstOrDefaultAsync(
            new ExpressionSpecification<ClientSubscription>(x => x.Id == client.ClientSubscriptionId),
            cancellationToken);

        if (subscription == null)
        {
            _logger.LogWarning("Subscription {SubscriptionId} not found for client {ClientId}",
                client.ClientSubscriptionId, client.Id);
            throw new NotFoundException(_localizer["Subscription not found"]);
        }

        return subscription;
    }

    private void ValidateSubscriptionStatus(Client client)
    {
        if (client.SubscriptionStatus != SubscriptionStatus.Active)
        {
            _logger.LogWarning("Client {ClientId} has inactive subscription", client.Id);
            throw new ForbiddenException(_localizer["Subscription is not active"]);
        }
    }

    private async Task<ClientOrder> CreateOrderAsync(
        CreateClientOrderRequest request,
        Client client,
        ClientSubscription subscription,
        CancellationToken cancellationToken)
    {
        var order = new ClientOrder
        {
            ClientId = client.Id,
            ClientSubscriptionId = subscription.Id,
            StartDate = request.Date,
            EndDate = request.Date,
            PaymentStatus = PaymentStatus.Pending,
            IsRecurring = request.IsRecurring
        };

        if (request.IsRecurring)
        {
            order.EndDate = subscription.EndDate;
        }

        var (totalCost, mealDescription) = await ProcessOrderItemsAsync(request, order, cancellationToken);
        order.TotalCost = totalCost;

        _logger.LogDebug("Created order with total cost {TotalCost}", totalCost);
        return order;
    }

    private async Task<(decimal TotalCost, string MealDescription)> ProcessOrderItemsAsync(
        CreateClientOrderRequest request,
        ClientOrder order,
        CancellationToken cancellationToken)
    {
        decimal totalCost = 0;
        var mealItems = new List<string>();
        int days = 1; // Default to 1 day for non-recurring orders

        if (request.IsRecurring)
        {
            days = (order.EndDate.DayNumber - order.StartDate.DayNumber) + 1;
            _logger.LogDebug("Recurring order calculated for {Days} days", days);
        }

        var mealDescription = $"Order ({days} {(days > 1 ? "days" : "day")}): ";

        foreach (var mealItem in request.Meals.Where(x => x.QuantityPerDay > 0))
        {
            var meal = await _mealRepo.FirstOrDefaultAsync(
                new ExpressionSpecification<Meal>(x => x.Id == mealItem.MealId),
                cancellationToken);

            if (meal == null || !meal.IsAddOn || meal.PriceForCustomer <= 0)
            {
                _logger.LogWarning("Invalid meal {Meal} in order request", meal?.NameEn ?? mealItem.MealId.ToString());
                throw new BadRequestException(_localizer["Only add-on meals can be requested"]);
            }

            order.ClientOrderItems.Add(new ClientOrderItem
            {
                MealId = mealItem.MealId,
                Price = meal.PriceForCustomer,
                QuantityPerDay = mealItem.QuantityPerDay
            });

            mealItems.Add($"{mealItem.QuantityPerDay}x {meal.NameEn}");
            totalCost += mealItem.QuantityPerDay * (decimal)meal.PriceForCustomer;
        }

        if (request.IsRecurring)
        {
            totalCost *= days;
            _logger.LogDebug("Recurring order calculated for {Days} days", days);
        }

        mealDescription += string.Join(", ", mealItems);
        order.MealDescription = mealDescription;
        return (totalCost, mealDescription);
    }

    private async Task<PaymentIntentResponseDTO> ProcessPaymentAsync(
        CreateClientOrderRequest request,
        ClientOrder order,
        Client client,
        ClientSubscription subscription,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.PaymentMethodId))
        {
            _logger.LogWarning("No payment method provided for order {OrderId}", order.Id);
            throw new BadRequestException(_localizer["Payment method is required"]);
        }

        var paymentIntent = await _stripeService.CreateAddOnPaymentLink(
            order.Id.ToString(),
            request.PaymentMethodId,
            client.StripeId,
            order.TotalCost,
            order.MealDescription);

        if (paymentIntent.Status != "succeeded")
        {
            _logger.LogWarning("Payment failed for order {OrderId} with status {Status}",
                order.Id, paymentIntent.Status);
            throw new BadRequestException(_localizer["Payment processing failed"]);
        }

        var payment = CreatePaymentRecord(client, subscription, paymentIntent, order.MealDescription);
        await _paymentRepo.AddAsync(payment, withSaveChanges: false, cancellationToken: cancellationToken);
        _logger.LogInformation("Created payment record {PaymentId}", payment.Id);

        await AddLoyaltyPoints(client, payment, order.MealDescription, cancellationToken);
        order.PaymentStatus = PaymentStatus.Paid;

        return paymentIntent;
    }

    private async Task AddLoyaltyPoints(
        Client client,
        Payment payment,
        string? mealDescription,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Calculating loyalty points for payment {PaymentId}", payment.Id);

        var points = await _clientPortalSettingservice.GetOnePointEqualInMoney();
        var pointsEarned = points == 0 ? 0 : (int)(payment.Amount / points);

        await _clientLoyaltyPointRepo.AddAsync(new ClientLoyaltyPoint
        {
            ClientId = client.Id,
            Reason = mealDescription,
            Source = "Payment",
            TransactionId = payment.Id.ToString(),
            Points = pointsEarned,
            Type = TransactionType.Earn,
            DateEarned = SystemTime.Now(),
        }, withSaveChanges: false, cancellationToken: cancellationToken);

        _logger.LogInformation("Awarded {PointsEarned} loyalty points to client {ClientId}",
            pointsEarned, client.Id);
    }

    private Payment CreatePaymentRecord(
        Client client,
        ClientSubscription subscription,
        PaymentIntentResponseDTO paymentIntent,
        string? mealDescription)
    {
        _logger.LogDebug("Creating payment record for payment intent {PaymentIntentId}", paymentIntent.Id);

        return new Payment
        {
            ClientId = client.Id,
            ClientSubscriptionId = subscription.Id,
            InvoiceNumber = paymentIntent.Id,
            TransactionId = paymentIntent.LatestChargeId ?? paymentIntent.Id,
            PaymentStatus = PaymentStatus.Paid,
            PaymentMethod = paymentIntent.PaymentMethodType ?? "unknown",
            Amount = paymentIntent.Amount / 100m,
            PaymentGateway = "Stripe",
            Reason = mealDescription,
            Currency = paymentIntent.Currency,
            PaymentDate = SystemTime.Now(),
            OrderId = paymentIntent.OrderId
        };
    }
}
