using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Domain.Common.ValueObjects;
using ZeroFat.ClientPortal.Domain.NutritionTracking;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.ClientPortal.Infrastructure.Persistence.Context;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.ClientPortal.Infrastructure.Services;

public class CalorieTrackingService : ICalorieTrackingService
{
    private readonly ClientPortalContext _dbContext;
    private readonly IRepository<MealType> _mealTypeRepo;
    private readonly ILogger<CalorieTrackingService> _logger;

    public CalorieTrackingService(
        ClientPortalContext dbContext,
        IRepository<MealType> mealTypeRepo,
        ILogger<CalorieTrackingService> logger)
    {
        _dbContext = dbContext;
        _mealTypeRepo = mealTypeRepo;
        _logger = logger;
    }

    public async Task RecordMealTimeCalories(Guid mealTypeId)
    {
        DateOnly date = DateOnly.FromDateTime(DateTime.UtcNow);
        using var _ = _logger.BeginScope("Recording calories for meal type {MealTypeId} on {Date}", mealTypeId, date);

        try
        {
            // Get the meal type name for the record
            var mealType = await _mealTypeRepo.FirstOrDefaultAsync(
                new ExpressionSpecification<MealType>(x => x.Id == mealTypeId));

            var mealTypeName = mealType?.NameEn ?? "Unknown Meal Type";

            // Process in batches of 25
            int batchSize = 25;
            int offset = 0;
            bool moreRecords = true;

            while (moreRecords)
            {
                // Get batch of daily selections
                var selections = await _dbContext.DailySelections
                    .Include(ds => ds.DailyMealSelections)
                        .ThenInclude(dms => dms.Meal)
                    .Where(x => x.Date == date &&
                                x.DailyMealSelections.Any(m => m.MealTypeId == mealTypeId && !m.IsConsumed))
                    .OrderBy(x => x.ClientId)  // Ensure consistent ordering
                    .Skip(offset)
                    .Take(batchSize)
                    .ToListAsync();

                if (selections.Count == 0)
                {
                    moreRecords = false;
                    continue;
                }

                // Process the current batch
                await ProcessBatch(selections, date, mealTypeId, mealTypeName);

                offset += batchSize;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording calories for meal type {MealTypeId} on {Date}", mealTypeId, date);
            throw;
        }
    }

    private async Task ProcessBatch(List<DailySelection> selections, DateOnly date, Guid mealTypeId, string mealTypeName)
    {
        // Prepare batch updates
        var calorieRecordsToAdd = new List<CalorieRecord>();
        var dailyHealthLogsToUpdate = new List<DailyHealthLog>();
        var dailyMealSelectionsToUpdate = new List<DailyMealSelection>();

        foreach (var selection in selections)
        {
            var clientId = selection.ClientId;
            var mealsForType = selection.DailyMealSelections
                .Where(m => m.MealTypeId == mealTypeId)
                .ToList();

            // Calculate totals
            var totalCalories = mealsForType.Sum(m => m.TotalCalories);
            var totalProtein = mealsForType.Sum(m => m.TotalProteins);
            var totalCarbs = mealsForType.Sum(m => m.TotalCarbohydrates);
            var totalFats = mealsForType.Sum(m => m.TotalFats);

            // Get meal names for description
            var mealNames = mealsForType
                .Where(m => m.Meal != null)
                .Select(m => m.Meal!.NameEn)
                .Distinct()
                .ToList();

            var description = mealNames.Any()
                ? $"Consumed {string.Join(", ", mealNames)}"
                : $"Meal type {mealTypeName}";

            // Mark meals as consumed
            foreach (var mealSelection in mealsForType)
            {
                mealSelection.IsConsumed = true;
                dailyMealSelectionsToUpdate.Add(mealSelection);
            }

            // Find or create health log
            var stat = await _dbContext.DailyHealthLogs
                .Include(dhl => dhl.CalorieRecords)
                .FirstOrDefaultAsync(x => x.LogDate == date && x.ClientId == clientId);

            if (stat != null)
            {
                var existingRecord = stat.CalorieRecords
                    .FirstOrDefault(c => c.Name == $"{mealTypeName} - {description}" && c.RecordType == CalorieRecordType.Food);

                if (existingRecord == null)
                {
                    // Add new record
                    var caloriesRecord = new CalorieRecord()
                    {
                        Calories = totalCalories,
                        DailyHealthLogId = stat.Id,
                        Name = $"{mealTypeName} - {description}",
                        RecordedAt = DateTime.UtcNow,
                        RecordType = CalorieRecordType.Food,
                        Nutrition = new NutritionFacts(
                            totalProtein,
                            totalCarbs,
                            totalFats,
                            totalCalories),
                    };

                    calorieRecordsToAdd.Add(caloriesRecord);
                    stat.TotalCaloriesConsumed += totalCalories;
                    dailyHealthLogsToUpdate.Add(stat);
                }
            }
            else
            {
                stat = new DailyHealthLog
                {
                    ClientId = clientId,
                    LogDate = date,
                    TotalCaloriesConsumed = totalCalories
                };
                stat.RecordWaterIntake(new WaterIntake(0));

                var caloriesRecord = new CalorieRecord()
                {
                    Calories = totalCalories,
                    DailyHealthLogId = stat.Id,
                    Name = $"{mealTypeName} - {description}",
                    RecordedAt = DateTime.UtcNow,
                    RecordType = CalorieRecordType.Food,
                    Nutrition = new NutritionFacts(
                        totalProtein,
                        totalCarbs,
                        totalFats,
                        totalCalories),
                };

                stat.AddCalorieRecord(caloriesRecord);
                await _dbContext.DailyHealthLogs.AddAsync(stat);
            }
        }

        // Perform batch updates for this batch
        if (dailyMealSelectionsToUpdate.Count != 0)
        {
            _dbContext.DailyMealSelections.UpdateRange(dailyMealSelectionsToUpdate);
        }

        if (calorieRecordsToAdd.Count != 0)
        {
            await _dbContext.CalorieRecords.AddRangeAsync(calorieRecordsToAdd);
        }

        if (dailyHealthLogsToUpdate.Count != 0)
        {
            _dbContext.DailyHealthLogs.UpdateRange(dailyHealthLogsToUpdate);
        }

        await _dbContext.SaveChangesAsync();
    }
}
