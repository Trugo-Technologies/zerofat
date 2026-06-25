using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZeroFat.NutriPlan.Domain.Settings;
using ZeroFat.NutriPlan.Infrastructure.Persistence.Context;
using ZeroFat.NutriPlan.Infrastructure.Services;

namespace ZeroFat.NutriPlan.Infrastructure.Seedings.NutriPlan;
public class AllergensSeeder : INutriPlanSeeder
{
    private readonly NutriPlanContext _db;
    private readonly ILogger<AllergensSeeder> _logger;

    public AllergensSeeder(ILogger<AllergensSeeder> logger, NutriPlanContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (!await _db.Allergens.AnyAsync(cancellationToken))
        {
            try
            {
                _logger.LogInformation("Started to Seed Allergens.");
                string allergensData = await File.ReadAllTextAsync(path + "/Seedings/NutriPlan/allergens.json", cancellationToken);
                var allergens = JsonSerializer.Deserialize<List<Allergen>>(allergensData);

                if (allergens != null)
                {
                    // foreach (var allergen in allergens)
                    // {
                    //     allergen.IsActive = true;
                    // }
                    await _db.Allergens.AddRangeAsync(allergens, cancellationToken);
                    await _db.SaveChangesAsync(cancellationToken);
                }
                _logger.LogInformation("Seed NutriPlan allergens Done.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Faild Seeding NutriPlan allergens.");
            }
        }
    }
}
