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
public class SubscriptionPlansSeeder : ICoreSeeder
{
    private readonly CoreContext _db;
    private readonly ILogger<SubscriptionPlansSeeder> _logger;

    public SubscriptionPlansSeeder(ILogger<SubscriptionPlansSeeder> logger, CoreContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (!await _db.SubscriptionPlans.AnyAsync(cancellationToken))
        {
            try
            {
                _logger.LogInformation("Started to Seed Subscription plans.");
                string SubscriptionPlansData = await File.ReadAllTextAsync(path + "/Seedings/Core/SubscriptionPlans.json", cancellationToken);
                var SubscriptionPlans = JsonSerializer.Deserialize<List<SubscriptionPlan>>(SubscriptionPlansData);

                if (SubscriptionPlans != null)
                {
                    await _db.SubscriptionPlans.AddRangeAsync(SubscriptionPlans, cancellationToken);
                    await _db.SaveChangesAsync(cancellationToken);
                }
                _logger.LogInformation("Seed Subscription plans Done.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Faild Seeding Subscription plans.");
            }
        }
    }
}
