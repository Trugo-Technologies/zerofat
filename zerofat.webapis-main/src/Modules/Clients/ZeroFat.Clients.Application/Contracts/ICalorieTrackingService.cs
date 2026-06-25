using Hangfire;
using ZeroFat.Application.Common.Interfaces;

namespace ZeroFat.ClientPortal.Application.Contracts;

public interface ICalorieTrackingService : ITransientService
{
    [Queue("calories-recorder")]
    [AutomaticRetry(Attempts = 0)] // No retries for data consistency
    Task RecordMealTimeCalories(Guid mealTypeId);
}

