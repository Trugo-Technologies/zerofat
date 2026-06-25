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
        if (!await _db.Settings.AnyAsync(cancellationToken))
        {
            try
            {
                _logger.LogInformation("Started to Seed NutriPlan Settings.");
                string counrtyData = await File.ReadAllTextAsync(path + "/Seedings/NutriPlan/nutriPlanSettings.json", cancellationToken);
                var nutriPlanSettings = JsonSerializer.Deserialize<List<Setting>>(counrtyData);

                if (nutriPlanSettings != null)
                {
                    await _db.Settings.AddRangeAsync(nutriPlanSettings, cancellationToken);
                    await _db.SaveChangesAsync(cancellationToken);
                }
                _logger.LogInformation("Seed NutriPlan Settings Done.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Faild Seeding NutriPlan Settings.");
            }
        }
    }
}
