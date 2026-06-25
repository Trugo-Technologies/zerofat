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
public class AdvertisementsSeeder : ICoreSeeder
{
    private readonly CoreContext _db;
    private readonly ILogger<AdvertisementsSeeder> _logger;

    public AdvertisementsSeeder(ILogger<AdvertisementsSeeder> logger, CoreContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (!await _db.Advertisements.AnyAsync(cancellationToken))
        {
            try
            {
                _logger.LogInformation("Started to Seed Advertisements.");
                string advertisementsData = await File.ReadAllTextAsync(path + "/Seedings/Core/advertisements.json", cancellationToken);
                var advertisements = JsonSerializer.Deserialize<List<Advertisement>>(advertisementsData);

                if (advertisements != null)
                {
                    foreach (var advertisement in advertisements)
                    {
                        advertisement.IsActive = true;
                    }
                    await _db.Advertisements.AddRangeAsync(advertisements, cancellationToken);
                    await _db.SaveChangesAsync(cancellationToken);
                }
                _logger.LogInformation("Seed Advertisements Done.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Faild Seeding Advertisements.");
            }
        }
    }
}
