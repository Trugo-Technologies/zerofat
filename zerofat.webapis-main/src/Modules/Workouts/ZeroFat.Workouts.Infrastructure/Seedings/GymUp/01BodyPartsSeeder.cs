using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZeroFat.GymUp.Infrastructure.Persistence.Context;
using ZeroFat.GymUp.Infrastructure.Services;

namespace ZeroFat.GymUp.Infrastructure.Seedings.GymUp;
public class BodyPartsSeeder : IWorkoutSeeder
{
    private readonly GymUpContext _db;
    private readonly ILogger<BodyPartsSeeder> _logger;

    public BodyPartsSeeder(ILogger<BodyPartsSeeder> logger, GymUpContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (!await _db.BodyParts.AnyAsync(cancellationToken))
        {
            try
            {
                _logger.LogInformation("Started to Seed Workout BodyParts.");
                string bodyPartsData = await File.ReadAllTextAsync(path + "/Seedings/GymUp/01bodyParts.json", cancellationToken);
                var bodyParts = JsonSerializer.Deserialize<List<BodyPart>>(bodyPartsData);

                if (bodyParts != null)
                {
                    foreach (var bodyPart in bodyParts)
                    {
                        bodyPart.IsActive = true;
                    }

                    await _db.BodyParts.AddRangeAsync(bodyParts, cancellationToken);
                    await _db.SaveChangesAsync(cancellationToken);
                }
                _logger.LogInformation("Seed Workout BodyParts Done.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Faild Seeding Workout BodyParts.");
            }
        }
    }
}
