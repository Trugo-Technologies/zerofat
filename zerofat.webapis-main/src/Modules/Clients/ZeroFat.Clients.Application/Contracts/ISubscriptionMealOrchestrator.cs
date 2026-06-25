using Hangfire;
using ZeroFat.Application.Common.Interfaces;

namespace ZeroFat.ClientPortal.Application.Contracts;
public interface ISubscriptionMealOrchestrator : ITransientService
{
    // Subscription creation methods
    [Queue("subscription-meals")]
    [AutomaticRetry(Attempts = 2)]
    Task CreateDailySelectionsForSubscriptionAsync(Guid subscriptionId);

    // Default meal assignment methods
    [Queue("subscription-meals")]
    [AutomaticRetry(Attempts = 0)] // No retries for data consistency
    Task AssignDefaultMealsToSubscriptionsAsync();


    [Queue("subscription-meals")]
    [AutomaticRetry(Attempts = 0)] // No retries for data consistency
    Task CreateDailyMealSelectionsForOrderAsync(Guid orderId);


    [Queue("subscription-meals")]
    [AutomaticRetry(Attempts = 0)]
    Task SyncSubscriptionStatusesAsync();
}

