using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZeroFat.Domain.Core;
using ZeroFat.Infrastructure.Core.Persistence.Context;
using ZeroFat.Infrastructure.Core.Services;
using ZeroFat.NutriPlan.Domain.Settings;
using ZeroFat.NutriPlan.Infrastructure.Persistence.Context;
using ZeroFat.NutriPlan.Infrastructure.Services;

namespace ZeroFat.NutriPlan.Infrastructure.Seedings.NutriPlan;
public class MealPlansSeeder : INutriPlanSeeder
{
    private readonly NutriPlanContext _db;
    private readonly ILogger<MealPlansSeeder> _logger;

    public MealPlansSeeder(ILogger<MealPlansSeeder> logger, NutriPlanContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (!await _db.MealTypes.AnyAsync(cancellationToken))
        {
            try
            {
                _logger.LogInformation("Started to Seed NutriPlan Settings.");
                string mealTypesData = await File.ReadAllTextAsync(path + "/Seedings/NutriPlan/mealTypes.json", cancellationToken);
                var mealTypes = JsonSerializer.Deserialize<List<MealType>>(mealTypesData);

                if (mealTypes != null)
                {
                    foreach (var mealType in mealTypes)
                    {
                        mealType.IsActive = true;
                    }
                    await _db.MealTypes.AddRangeAsync(mealTypes, cancellationToken);
                    await _db.SaveChangesAsync(cancellationToken);
                }
                _logger.LogInformation("Seed NutriPlan MealTypes Done.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Faild Seeding NutriPlan Settings.");
            }
        }

        if (!await _db.MealPlans.AnyAsync(cancellationToken))
        {
            try
            {
                var mealTypes = await _db.MealTypes.ToListAsync(cancellationToken);
                _logger.LogInformation("Started to Seed NutriPlan Settings.");
                string mealPlansData = await File.ReadAllTextAsync(path + "/Seedings/NutriPlan/mealPlans.json", cancellationToken);
                var mealPlans = JsonSerializer.Deserialize<List<MealPlan>>(mealPlansData);

                if (mealPlans != null)
                {
                    foreach (var mealPlan in mealPlans)
                    {
                        mealPlan.IsActive = true;
                        foreach (var mealType in mealTypes)
                        {
                            var random1 = new Random().Next(20, 40);
                            var random2 = new Random().Next(30, 50);
                            mealPlan.MealPlanMealTypes.Add(new MealPlanMealType()
                            {
                                Price = random1 * 3,
                                AverageCalories = random2 * 10,
                                MealTypeId = mealType.Id,
                            });
                        }
                    }
                    await _db.MealPlans.AddRangeAsync(mealPlans, cancellationToken);
                    await _db.SaveChangesAsync(cancellationToken);
                }
                _logger.LogInformation("Seed NutriPlan MealPlans Done.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Faild Seeding NutriPlan Settings.");
            }
        }

    }
}
