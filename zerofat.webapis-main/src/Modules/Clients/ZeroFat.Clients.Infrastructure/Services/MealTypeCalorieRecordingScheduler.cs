using Hangfire;
using Hangfire.Storage;
using Microsoft.Extensions.Logging;
using TimeZoneConverter;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.Infrastructure.BackgroundProcessing.Contracts;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.ClientPortal.Infrastructure.Services;

public class MealTypeCalorieRecordingScheduler : IRecurringBackgroundJobScheduler
{
    private readonly ICalorieTrackingService _calorieTrackingService;
    private readonly IRepository<MealType> _scheduleRepository;
    private readonly ILogger<MealTypeCalorieRecordingScheduler> _logger;

    public MealTypeCalorieRecordingScheduler(
        ICalorieTrackingService calorieTrackingService,
        IRepository<MealType> scheduleRepository,
        ILogger<MealTypeCalorieRecordingScheduler> logger)
    {
        _calorieTrackingService = calorieTrackingService;
        _scheduleRepository = scheduleRepository;
        _logger = logger;
    }

    public async Task ScheduleAsync(IRecurringJobManager recurringJobManager)
    {
        try
        {
            // Get UAE timezone

            var uaeTimeZone = TZConvert.GetTimeZoneInfo("Arabian Standard Time");

            // Get all active meal types from database
            var activeMealTypes = await _scheduleRepository.ListAsync(
                new ExpressionSpecification<MealType>(x => x.IsActive));

            // Clean up old jobs (as per previous implementation)
            CleanupOrphanedJobs(recurringJobManager, activeMealTypes);

            // Schedule jobs in UAE time
            foreach (var mealType in activeMealTypes)
            {
                var time = mealType.ScheduledTime;

                // Create cron expression in UTC that matches the UAE time
                var cronExpression = $"{time.Minutes} {time.Hours} * * *";

                recurringJobManager.AddOrUpdate(
                    $"calories-record-{mealType.NameEn}",
                    () => _calorieTrackingService.RecordMealTimeCalories(mealType.Id),
                    cronExpression,
                    new RecurringJobOptions()
                    {
                        TimeZone = uaeTimeZone,
                    }); // Pass UAE timezone directly

                _logger.LogInformation("Scheduled calorie recording for {MealType} at {Time} UAE time", mealType.NameEn, time.ToString(@"hh\:mm"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to schedule meal type calorie recording jobs");
            throw;
        }
    }

    private void CleanupOrphanedJobs(IRecurringJobManager recurringJobManager, List<MealType> activeMealTypes)
    {
        var existingJobs = JobStorage.Current.GetConnection().GetRecurringJobs()
            .Where(j => j.Id.StartsWith("calories-record-"))
            .ToList();

        foreach (var job in existingJobs)
        {
            var mealTypeName = job.Id.Replace("calories-record-", "");

            if (!activeMealTypes.Any(m => m.NameEn == mealTypeName))
            {
                recurringJobManager.RemoveIfExists(job.Id);
                _logger.LogInformation("Removed orphaned calorie recording job: {JobId}", job.Id);
            }
        }
    }
}
