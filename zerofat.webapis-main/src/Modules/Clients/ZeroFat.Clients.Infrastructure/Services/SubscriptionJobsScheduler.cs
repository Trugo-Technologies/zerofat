using Hangfire;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.Infrastructure.BackgroundProcessing.Contracts;

namespace ZeroFat.ClientPortal.Infrastructure.Services;

public class SubscriptionJobsScheduler : IRecurringBackgroundJobScheduler
{
    private readonly ISubscriptionMealOrchestrator _subscriptionMealOrchestrator;

    public SubscriptionJobsScheduler(
        ISubscriptionMealOrchestrator subscriptionMealOrchestrator)
    {
        _subscriptionMealOrchestrator = subscriptionMealOrchestrator;
    }

    public void Schedule(IRecurringJobManager recurringJobManager)
    {
        // Run daily at 2 AM UTC
        recurringJobManager.AddOrUpdate(
            "subscription-sync",
            () => _subscriptionMealOrchestrator.SyncSubscriptionStatusesAsync(),
            Cron.Daily(2),
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Utc
            });
    }
}
