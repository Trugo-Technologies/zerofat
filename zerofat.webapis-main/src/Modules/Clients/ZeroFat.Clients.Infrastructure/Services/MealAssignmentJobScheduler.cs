using Hangfire;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.Infrastructure.BackgroundProcessing.Contracts;

namespace ZeroFat.ClientPortal.Infrastructure.Services;

public class MealAssignmentJobScheduler : IRecurringBackgroundJobScheduler
{
    private readonly ISubscriptionMealOrchestrator _mealAssignmentService;

    public MealAssignmentJobScheduler(
        ISubscriptionMealOrchestrator mealAssignmentService)
    {
        _mealAssignmentService = mealAssignmentService;
    }

    public Task ScheduleAsync(IRecurringJobManager recurringJobManager)
    {
        recurringJobManager.AddOrUpdate(
            "default-meal-assignment-daily",
            () => _mealAssignmentService.AssignDefaultMealsToSubscriptionsAsync(),
            Cron.Daily(3)); // 3 AM UTC
        return Task.CompletedTask;
    }
}
