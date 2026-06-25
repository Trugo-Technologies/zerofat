using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZeroFat.ClientPortal.Infrastructure.Services;
using ZeroFat.Domain.Core;
using ZeroFat.Domain.Enums;
using ZeroFat.Infrastructure.Core.Persistence.Context;

namespace ZeroFat.ClientPortal.Infrastructure.Seedings.ClientPortal;
public class ClientPortalSettingsSeeder : IClientPortalSeeder
{
    private readonly CoreContext _db;
    private readonly ILogger<ClientPortalSettingsSeeder> _logger;

    public ClientPortalSettingsSeeder(ILogger<ClientPortalSettingsSeeder> logger, CoreContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        try
        {
            _logger.LogInformation("Started to Seed ClientPortal Settings.");
            string counrtyData = await File.ReadAllTextAsync(path + "/Seedings/ClientPortal/clientPortalSettings.json", cancellationToken);
            var nutriPlanSettings = JsonSerializer.Deserialize<List<Setting>>(counrtyData);

            if (nutriPlanSettings != null)
            {
                var existingPropertyNames = (await _db.Settings
                    .Where(s => s.ApplicationModule == ApplicationModule.ClientPortal)
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

            _logger.LogInformation("Seed ClientPortal Settings Done.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Faild Seeding ClientPortal Settings.");
        }
    }
}
