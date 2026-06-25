using ZeroFat.Application.Common.Models;
using ZeroFat.Domain.Enums;

namespace ZeroFat.Application.Common.Interfaces;

public interface IStripeService : ITransientService
{
    Task<string?> CreateCustomerOnStripe(string? email, string? name, string? phone, string referenceId);
    Task<string?> CreateProductOnStripe(string? code, string? name, string referenceId);
    Task<StrripeSessionInfo?> GetStripeSeesionInfoAsync(string sessionId);

    Task<string?> CreateSetupIntentAsync(string customerId);

    Task<StripeInvoiceDto> AdvanceFirstPaymentAsync(string subscriptionId, string customerId);

    Task<(string? id, string? url)> CreateSubscriptionLink(
        string subscriptionId,
        string customerId,
        string productId,
        decimal amount,
        string mealSelections,
        string productName,
        string description,
        DefaultIdType mealPlanId,
        List<DayOfWeek> days,
        bool isRecurring,
        SubscriptionType subscriptionType,
        DateOnly startDate,
        DateOnly endDate,
        int? offsetSubscription,
        string? couponCode);

    Task<(string? id, string? url)> CreateSubscriptionInvoice(
       string subscriptionId,
       string customerId,
       string productId,
       decimal amount,
       string mealSelections,
       string productName,
       string description,
       DefaultIdType mealPlanId,
       List<DayOfWeek> days,
       bool isRecurring,
       SubscriptionType subscriptionType,
       DateOnly startDate,
       DateOnly endDate,
       int? offsetSubscription,
       string? couponCode
       );


    Task<StripeSubscriptionInfo?> GetStripeSubscriptionInfoAsync(string subscriptionId);

    Task<PaymentIntentResponseDTO> CreateAddOnPaymentLink(
        string orderId,
        string paymentMethodId,
        string customerId,
        decimal amount,
        string mealDescription);

    Task<string?> UpdateCustomerOnStripe(string customerId, string? email, string? name, string? phone, string referenceId);


    Task<StripePaymentMethodDto?> AttachPaymentMethodToCustomer(string cardId, string customerId);
    Task<bool> MakeDefaultPaymentMethod(string cardId, string customerId);
    Task<bool> DetachPaymentMethodFromCustomer(string id);
    Task<List<StripePaymentMethodDto>?> GetCustomerPaymentMethods(string customerId);

    Task<StripeCouponDto?> GetCouponByCodeAsync(string couponCode);
    Task<string?> CreateDiscountRuleOnStripeAsync(StripeCouponDto couponDto);
}
