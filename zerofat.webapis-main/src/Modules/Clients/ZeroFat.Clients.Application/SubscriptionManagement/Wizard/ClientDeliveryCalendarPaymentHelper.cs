using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.Wizard;

internal static class ClientDeliveryCalendarPaymentHelper
{
    public static async Task<string> ResolvePaymentMethodIdAsync(
        DefaultIdType clientId,
        string? paymentMethodId,
        bool waivePayment,
        decimal amount,
        IReadRepository<ClientPaymentMethod> paymentMethodRepo,
        IStringLocalizer localizer,
        CancellationToken cancellationToken)
    {
        if (amount <= 0 || waivePayment)
        {
            return paymentMethodId ?? string.Empty;
        }

        if (!string.IsNullOrWhiteSpace(paymentMethodId))
        {
            return paymentMethodId;
        }

        var defaultMethod = await paymentMethodRepo.FirstOrDefaultAsync(
            new ExpressionSpecification<ClientPaymentMethod>(x => x.ClientId == clientId && x.IsDefault),
            cancellationToken);

        if (defaultMethod?.StripeId == null)
        {
            throw new BadRequestException(localizer["Payment method is required to complete this charge."]);
        }

        return defaultMethod.StripeId;
    }

    public static async Task<ClientDeliveryPaymentResultDto> ChargeAsync(
        Client client,
        ClientSubscription subscription,
        decimal amount,
        string description,
        string referenceId,
        string? paymentMethodId,
        bool waivePayment,
        IStripeService stripeService,
        IRepository<Payment> paymentRepo,
        IClientPortalSettingservice settings,
        IRepository<ClientLoyaltyPoint> loyaltyRepo,
        IStringLocalizer localizer,
        CancellationToken cancellationToken)
    {
        var result = new ClientDeliveryPaymentResultDto
        {
            AmountDue = amount,
            PaymentWaived = waivePayment && amount > 0,
            RequiresPayment = amount > 0 && !waivePayment
        };

        if (amount <= 0)
        {
            return result;
        }

        if (waivePayment)
        {
            return result;
        }

        if (string.IsNullOrWhiteSpace(client.StripeId))
        {
            throw new BadRequestException(localizer["Client does not have a Stripe customer profile."]);
        }

        if (string.IsNullOrWhiteSpace(paymentMethodId))
        {
            throw new BadRequestException(localizer["Payment method is required to complete this charge."]);
        }

        var paymentIntent = await stripeService.CreateAddOnPaymentLink(
            referenceId,
            paymentMethodId,
            client.StripeId,
            amount,
            description);

        if (paymentIntent.Status != "succeeded")
        {
            throw new BadRequestException(localizer["Payment processing failed"]);
        }

        var payment = new Payment
        {
            ClientId = client.Id,
            ClientSubscriptionId = subscription.Id,
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

        await paymentRepo.AddAsync(payment, cancellationToken);
        await AddLoyaltyPointsAsync(client, payment, description, settings, loyaltyRepo, cancellationToken);

        result.AmountCharged = payment.Amount;
        result.PaymentId = payment.Id;
        result.RequiresPayment = false;

        return result;
    }

    private static async Task AddLoyaltyPointsAsync(
        Client client,
        Payment payment,
        string? reason,
        IClientPortalSettingservice settings,
        IRepository<ClientLoyaltyPoint> loyaltyRepo,
        CancellationToken cancellationToken)
    {
        var pointsPerAed = await settings.GetOnePointEqualInMoney();
        var pointsEarned = pointsPerAed == 0 ? 0 : (int)(payment.Amount / pointsPerAed);

        if (pointsEarned <= 0)
        {
            return;
        }

        await loyaltyRepo.AddAsync(new ClientLoyaltyPoint
        {
            ClientId = client.Id,
            Reason = reason,
            Source = "Payment",
            TransactionId = payment.Id.ToString(),
            Points = pointsEarned,
            Type = TransactionType.Earn,
            DateEarned = DateTime.UtcNow
        }, cancellationToken);
    }
}
