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

    public Task ScheduleAsync(IRecurringJobManager recurringJobManager)
    {
        recurringJobManager.AddOrUpdate(
            "subscription-sync",
            () => _subscriptionMealOrchestrator.SyncSubscriptionStatusesAsync(),
            Cron.Daily(2),
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Utc
            });
        return Task.CompletedTask;
    }
}
