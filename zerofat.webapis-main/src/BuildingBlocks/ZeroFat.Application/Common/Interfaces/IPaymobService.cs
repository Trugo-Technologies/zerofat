
using ZeroFat.Shared.Paymob;

namespace ZeroFat.Application.Common.Interfaces;

public interface IPaymobService : ITransientService
{
    Task<string?> GetToken();

    Task<(string? id, string? url, string? order)> GetPaymentQuicklink(string email, string phone, string fullName, decimal amount, List<PaymobItem>? items = null, string? description = default);

    Task<string> CreateSubsciptionPlan(int frequency, string type);
    Task<string> UpdateSubsciptionPlan(int id, int numberOfDeduction, int amountInCents, int integration);
    Task ListSubsciptionPlan();
    Task<bool> SuspendSubsciptionPlan(int id);
    Task<bool> ResumeSubsciptionPlan(int id);
    Task<(string? id, string? url)> CreateSubscriptionLink(string email, string phone, string fullName, string? fullNameEn, decimal amount, int? subscriptionPlanId, string planName, string startDate, List<PaymobItem> items);
    Task<bool> SuspendSubsciption(string id);
    Task<bool> ResumeSubsciption(string id);
    Task<bool> CancelSubsciption(string id);
    Task<bool> UpdateSubsciption(string id, int amountCents, string endsAt);
    Task GetSubsciption(string id);

    Task test(string id);
}
