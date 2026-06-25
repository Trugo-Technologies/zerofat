using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZeroFat.Domain.Core;
using ZeroFat.Domain.Enums;
using ZeroFat.Infrastructure.Core.Persistence.Context;
using ZeroFat.NutriPlan.Infrastructure.Services;

namespace ZeroFat.NutriPlan.Infrastructure.Seedings.NutriPlan;
public class NutriPlanSettingsSeeder : INutriPlanSeeder
{
    private readonly CoreContext _db;
    private readonly ILogger<NutriPlanSettingsSeeder> _logger;

    public NutriPlanSettingsSeeder(ILogger<NutriPlanSettingsSeeder> logger, CoreContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        try
        {
            _logger.LogInformation("Started to Seed NutriPlan Settings.");
            string settingsData = await File.ReadAllTextAsync(path + "/Seedings/NutriPlan/nutriPlanSettings.json", cancellationToken);
            var nutriPlanSettings = JsonSerializer.Deserialize<List<Setting>>(settingsData);

            if (nutriPlanSettings != null)
            {
                var existingPropertyNames = (await _db.Settings
                    .Where(s => s.ApplicationModule == ApplicationModule.NutriPlan)
                    .Select(s => s.PropertyName)
                    .ToListAsync(cancellationToken))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                var settingsToAdd = nutriPlanSettings
                    .Where(s => !existingPropertyNames.Contains(s.PropertyName))
                    .ToList();

                if (settingsToAdd.Count != 0)
                {
                    await _db.Settings.AddRangeAsync(settingsToAdd, cancellationToken);
                    await _db.SaveChangesAsync(cancellationToken);
                }
            }

            _logger.LogInformation("Seed NutriPlan Settings Done.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Faild Seeding NutriPlan Settings.");
        }
    }
}
