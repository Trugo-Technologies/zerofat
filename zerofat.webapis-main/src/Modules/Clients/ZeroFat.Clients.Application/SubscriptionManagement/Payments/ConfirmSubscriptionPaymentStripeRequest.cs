using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.Domain.Common;
using ZeroFat.Domain.Enums;
using ZeroFat.Shared.Paymob;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.Payments;

public class ConfirmSubscriptionPaymentStripeRequest : ICommand<Result<bool>>
{
    public string? SessionId { get; set; }
    public string? SubscriptionId { get; set; }
    public ConfirmSubscriptionPaymentStripeRequest(string? sessionId, string? subscriptionId)
    {
        SubscriptionId = subscriptionId;
        SessionId = sessionId;
    }
}

public class ConfirmSubscriptionPaymentStripeRequestHandler(
    IRepositoryWithEvents<Payment> repository,
    IRepository<Client> clientRepo,
    IRepository<ClientSubscription> clientSubscriptionRepo,
    IRepository<ClientPaymentMethod> clientPaymentMethodRepo,
    IRepositoryWithEvents<ClientLoyaltyPoint> clientLoyaltyPointRepo,
    IStripeService stripeService,
    IClientPortalSettingservice clientPortalSettingservice,
    IJobService jobService,
    IStringLocalizer<ConfirmSubscriptionPaymentStripeRequestHandler> localizer,
    ILogger<ConfirmSubscriptionPaymentStripeRequestHandler> logger
    ) : IRequestHandler<ConfirmSubscriptionPaymentStripeRequest, Result<bool>>
{
    private readonly IRepositoryWithEvents<Payment> _repository = repository;
    private readonly IRepository<Client> _clientRepo = clientRepo;
    private readonly IRepository<ClientSubscription> _clientSubscriptionRepo = clientSubscriptionRepo;
    private readonly IRepositoryWithEvents<ClientLoyaltyPoint> _clientLoyaltyPointRepo = clientLoyaltyPointRepo;
    private readonly IStripeService _stripeService = stripeService;
    private readonly IJobService _jobService = jobService;
    private readonly IClientPortalSettingservice _clientPortalSettingservice = clientPortalSettingservice;
    private readonly ILogger<ConfirmSubscriptionPaymentStripeRequestHandler> _logger = logger;


    public async Task<Result<bool>> Handle(ConfirmSubscriptionPaymentStripeRequest request, CancellationToken cancellationToken)
    {
        using var loggerScope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["SubscriptionId"] = request.SubscriptionId ?? string.Empty,
            ["SessionId"] = request.SessionId ?? string.Empty,
            ["Operation"] = "ConfirmSubscriptionPayment"
        });

        try
        {
            _logger.LogInformation("Starting subscription payment confirmation");

            // 1. Retrieve subscription
            var clientSubscription = await _clientSubscriptionRepo.FirstOrDefaultAsync(
                new ExpressionSpecification<ClientSubscription>(x => x.Id.ToString() == request.SubscriptionId),
                cancellationToken);

            if (clientSubscription == null)
            {
                _logger.LogWarning("Subscription {SubscriptionId} not found", request.SubscriptionId);
                return await Result<bool>.FailAsync("Subscription not found");
            }

            if (clientSubscription.PaymentStatus == PaymentStatus.Paid)
            {
                _logger.LogInformation("Subscription already paid");
                return await Result<bool>.SuccessAsync("Subscription already paid");
            }

            // 2. Retrieve client
            var client = await _clientRepo.FirstOrDefaultAsync(
                new ExpressionSpecification<Client>(x => x.Id == clientSubscription.ClientId),
                cancellationToken);

            if (client == null)
            {
                _logger.LogError("Client {ClientId} not found for subscription", clientSubscription.ClientId);
                return await Result<bool>.FailAsync("Client not found");
            }

            // 3. Verify Stripe payment
            _logger.LogDebug("Retrieving Stripe session information");
            var sessionStripe = await _stripeService.GetStripeSeesionInfoAsync(request.SessionId);
            
            _logger.LogInformation("Stripe session details: {@StripeSession}", sessionStripe);

            if (sessionStripe?.PaymentStatus != "paid")
            {
                _logger.LogWarning("Payment not completed for session {SessionId}", request.SessionId);
                return await Result<bool>.FailAsync("Payment not completed");
            }

            //if (clientSubscription.IsRecurring)
            //{
            //    var invoiceDto = await _stripeService.AdvanceFirstPaymentAsync(sessionStripe.SubscriptionId, client.StripeId);
            //}

            // 4. Update subscription status
            _logger.LogInformation("Updating subscription status to Active");
            clientSubscription.SubscriptionStatus = SubscriptionStatus.Active;
            clientSubscription.PaymentStatus = PaymentStatus.Paid;
            clientSubscription.StripeSubscriptionId = sessionStripe.SubscriptionId;
            client.SubscriptionStatus = SubscriptionStatus.Active;

            // 5. Create payment record
            var payment = CreatePaymentRecord(client, clientSubscription, sessionStripe);
            await _repository.AddAsync(payment, withSaveChanges: false, cancellationToken: cancellationToken);
            _logger.LogInformation("Created payment record {PaymentId}", payment.Id);

            // 6. Add loyalty points
            await AddLoyaltyPoints(client, clientSubscription, payment, cancellationToken);
            _logger.LogInformation("Added loyalty points for client {ClientId}", client.Id);

            if(!await clientPaymentMethodRepo.AnyAsync(new ExpressionSpecification<ClientPaymentMethod>(x => x.CustomerId == client.StripeId!), cancellationToken))
            {
                var stripePaymentMethods = await _stripeService.GetCustomerPaymentMethods(client.StripeId!);
                if (stripePaymentMethods?.Count > 0) 
                {
                    var stripePaymentMethod = stripePaymentMethods.FirstOrDefault()!;
                    var paymentMethod = new ClientPaymentMethod
                    {
                        IsDefault = true,
                        StripeId = stripePaymentMethod.Id,
                        ClientId = client.Id,
                        Type = stripePaymentMethod.Type,
                        CustomerId = stripePaymentMethod.CustomerId,
                        CardBrand = stripePaymentMethod.Card?.Brand,
                        CardExpMonth = stripePaymentMethod.Card?.ExpMonth,
                        CardFunding = stripePaymentMethod.Card?.Funding,
                        CardLast4 = stripePaymentMethod.Card?.Last4,
                        CardName = stripePaymentMethod.Card?.Name
                    };

                    await clientPaymentMethodRepo.AddAsync(paymentMethod, cancellationToken);
                }
            }



            // 7. Save all changes
            await _repository.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("All changes saved successfully");

            // 8. Trigger meal generation
            _jobService.Enqueue<ISubscriptionMealOrchestrator>(
                x => x.CreateDailySelectionsForSubscriptionAsync(clientSubscription.Id));
            _logger.LogInformation("Queued meal generation job for subscription");

            return await Result<bool>.SuccessAsync("Payment confirmation done");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to confirm subscription payment");
            return await Result<bool>.FailAsync("Payment confirmation failed");
        }
    }

    private Payment CreatePaymentRecord(Client client, ClientSubscription subscription, StrripeSessionInfo session)
    {
        _logger.LogDebug("Creating payment record");

        string subscriptionDescription = subscription.SubscriptionType switch
        {
            SubscriptionType.OneWeek => "1-Week Meal Plan Subscription",
            SubscriptionType.OneMonth => "1-Month Meal Plan Subscription",
            SubscriptionType.ThreeMonths => "3-Month Meal Plan Subscription",
            _ => "Meal Plan Subscription"
        };

        return new Payment()
        {
            ClientId = client.Id,
            ClientSubscriptionId = subscription.Id,
            InvoiceNumber = session.PaymentIntentId,
            TransactionId = session.SessionId,
            PaymentStatus = PaymentStatus.Paid,
            PaymentMethod = string.Join(", ", session.PaymentMethodTypes),
            Amount = session.AmountTotal / (decimal)100,
            PaymentGateway = "Stripe",
            Reason = $"Payment for {subscriptionDescription}",
            Currency = session.Currency,
            PaymentDate = SystemTime.Now(),
            OrderId = session.SessionId,
            Notes = JsonSerializer.Serialize(session),
        };
    }

    private async Task AddLoyaltyPoints(Client client, ClientSubscription subscription, Payment payment, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Calculating loyalty points");

        var points = await _clientPortalSettingservice.GetOnePointEqualInMoney();
        var pointsEarned = points == 0 ? 0 : (int)(payment.Amount / points);

        string subscriptionDescription = subscription.SubscriptionType switch
        {
            SubscriptionType.OneWeek => "1-week subscription",
            SubscriptionType.OneMonth => "1-month subscription",
            SubscriptionType.ThreeMonths => "3-month subscription",
            _ => "subscription"
        };

        await _clientLoyaltyPointRepo.AddAsync(new ClientLoyaltyPoint()
        {
            ClientId = client.Id,
            Reason = $"Loyalty points for {subscriptionDescription} purchase",
            Source = "Payment",
            TransactionId = payment.Id.ToString(),
            Points = pointsEarned,
            Type = TransactionType.Earn,
            DateEarned = SystemTime.Now(),
        }, withSaveChanges: false, cancellationToken: cancellationToken);

        _logger.LogInformation("Awarded {PointsEarned} loyalty points", pointsEarned);
    }
}
