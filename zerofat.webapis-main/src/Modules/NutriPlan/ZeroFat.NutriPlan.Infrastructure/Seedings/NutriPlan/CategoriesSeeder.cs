using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZeroFat.NutriPlan.Domain.Settings;
using ZeroFat.NutriPlan.Infrastructure.Persistence.Context;
using ZeroFat.NutriPlan.Infrastructure.Services;

namespace ZeroFat.NutriPlan.Infrastructure.Seedings.NutriPlan;
public class CategoriesSeeder : INutriPlanSeeder
{
    private readonly NutriPlanContext _db;
    private readonly ILogger<CategoriesSeeder> _logger;

    public CategoriesSeeder(ILogger<CategoriesSeeder> logger, NutriPlanContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (!await _db.Categories.AnyAsync(cancellationToken))
        {
            try
            {
                _logger.LogInformation("Started to Seed Categories.");
                string categoriesData = await File.ReadAllTextAsync(path + "/Seedings/NutriPlan/categories.json", cancellationToken);
                var categories = JsonSerializer.Deserialize<List<Category>>(categoriesData);

                if (categories != null)
                {
                    // foreach (var allergen in categories)
                    // {
                    //     allergen.IsActive = true;
                    // }
                    await _db.Categories.AddRangeAsync(categories, cancellationToken);
                    await _db.SaveChangesAsync(cancellationToken);
                }
                _logger.LogInformation("Seed NutriPlan categories Done.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Faild Seeding NutriPlan categories.");
            }
        }
    }
}
