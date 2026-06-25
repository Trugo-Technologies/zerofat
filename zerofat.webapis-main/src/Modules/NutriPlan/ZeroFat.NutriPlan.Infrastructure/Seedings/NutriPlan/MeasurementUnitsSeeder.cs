using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZeroFat.NutriPlan.Domain.Settings;
using ZeroFat.NutriPlan.Infrastructure.Persistence.Context;
using ZeroFat.NutriPlan.Infrastructure.Services;

namespace ZeroFat.NutriPlan.Infrastructure.Seedings.NutriPlan;
public class MeasurementUnitsSeeder : INutriPlanSeeder
{
    private readonly NutriPlanContext _db;
    private readonly ILogger<MeasurementUnitsSeeder> _logger;

    public MeasurementUnitsSeeder(ILogger<MeasurementUnitsSeeder> logger, NutriPlanContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (!await _db.MeasurementUnits.AnyAsync(cancellationToken))
        {
            try
            {
                _logger.LogInformation("Started to Seed MeasurementUnits.");
                string measurementUnitsData = await File.ReadAllTextAsync(path + "/Seedings/NutriPlan/measurementUnits.json", cancellationToken);
                var measurementUnits = JsonSerializer.Deserialize<List<MeasurementUnit>>(measurementUnitsData);

                if (measurementUnits != null)
                {
                    // foreach (var allergen in allergens)
                    // {
                    //     allergen.IsActive = true;
                    // }
                    await _db.MeasurementUnits.AddRangeAsync(measurementUnits, cancellationToken);
                    await _db.SaveChangesAsync(cancellationToken);
                }
                _logger.LogInformation("Seed NutriPlan MeasurementUnits Done.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Faild Seeding NutriPlan MeasurementUnits.");
            }
        }
    }
}
