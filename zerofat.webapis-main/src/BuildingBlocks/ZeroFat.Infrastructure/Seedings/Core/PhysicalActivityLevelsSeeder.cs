using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZeroFat.Domain.Core;
using ZeroFat.Infrastructure.Core.Persistence.Context;
using ZeroFat.Infrastructure.Core.Services;

namespace ZeroFat.Infrastructure.Seedings.Core;
public class PhysicalActivityLevelsSeeder : ICoreSeeder
{
    private readonly CoreContext _db;
    private readonly ILogger<PhysicalActivityLevelsSeeder> _logger;

    public PhysicalActivityLevelsSeeder(ILogger<PhysicalActivityLevelsSeeder> logger, CoreContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (!await _db.PhysicalActivityLevels.AnyAsync(cancellationToken))
        {
            try
            {
                _logger.LogInformation("Started to Seed PhysicalActivityLevels.");
                string counrtyData = await File.ReadAllTextAsync(path + "/Seedings/Core/physicalActivityLevels.json", cancellationToken);
                var physicalActivityLevels = JsonSerializer.Deserialize<List<PhysicalActivityLevel>>(counrtyData);

                if (physicalActivityLevels != null)
                {
                    foreach (var physicalActivityLevel in physicalActivityLevels)
                    {
                        physicalActivityLevel.IsActive = true;
                    }
                    await _db.PhysicalActivityLevels.AddRangeAsync(physicalActivityLevels, cancellationToken);
                    await _db.SaveChangesAsync(cancellationToken);
                }
                _logger.LogInformation("Seed PhysicalActivityLevels Done.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Faild Seeding PhysicalActivityLevels.");
            }
        }
    }
}
