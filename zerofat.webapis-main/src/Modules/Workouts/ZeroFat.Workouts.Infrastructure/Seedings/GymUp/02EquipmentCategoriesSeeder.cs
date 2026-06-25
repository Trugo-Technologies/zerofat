using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZeroFat.GymUp.Infrastructure.Persistence.Context;
using ZeroFat.GymUp.Infrastructure.Services;

namespace ZeroFat.GymUp.Infrastructure.Seedings.GymUp;
public class EquipmentCategoriesSeeder : IWorkoutSeeder
{
    private readonly GymUpContext _db;
    private readonly ILogger<EquipmentCategoriesSeeder> _logger;

    public EquipmentCategoriesSeeder(ILogger<EquipmentCategoriesSeeder> logger, GymUpContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (!await _db.EquipmentCategories.AnyAsync(cancellationToken))
        {
            try
            {
                _logger.LogInformation("Started to Seed Workout EquipmentCategories.");
                string equipmentCategoriesData = await File.ReadAllTextAsync(path + "/Seedings/GymUp/02equipmentCategories.json", cancellationToken);
                var equipmentCategories = JsonSerializer.Deserialize<List<EquipmentCategory>>(equipmentCategoriesData);

                if (equipmentCategories != null)
                {
                    foreach (var equipmentCategory in equipmentCategories)
                    {
                        equipmentCategory.IsActive = true;
                    }

                    await _db.EquipmentCategories.AddRangeAsync(equipmentCategories, cancellationToken);
                    await _db.SaveChangesAsync(cancellationToken);
                }
                _logger.LogInformation("Seed Workout EquipmentCategories Done.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Faild Seeding Workout EquipmentCategories.");
            }
        }
    }
}
