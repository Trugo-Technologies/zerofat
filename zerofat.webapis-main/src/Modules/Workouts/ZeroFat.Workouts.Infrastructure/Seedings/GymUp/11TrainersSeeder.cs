using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZeroFat.GymUp.Infrastructure.Persistence.Context;
using ZeroFat.GymUp.Infrastructure.Services;

namespace ZeroFat.GymUp.Infrastructure.Seedings.GymUp;
public class TrainersSeeder : IWorkoutSeeder
{
    private readonly GymUpContext _db;
    private readonly ILogger<TrainersSeeder> _logger;

    public TrainersSeeder(ILogger<TrainersSeeder> logger, GymUpContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (!await _db.Trainers.AnyAsync(cancellationToken))
        {
            try
            {
                _logger.LogInformation("Started to Seed Workout Trainers.");
                string trainersData = await File.ReadAllTextAsync(path + "/Seedings/GymUp/11trainers.json", cancellationToken);
                var trainers = JsonSerializer.Deserialize<List<Trainer>>(trainersData);

                if (trainers != null)
                {
                    foreach (var trainer in trainers)
                    {
                        trainer.IsActive = true;
                    }

                    await _db.Trainers.AddRangeAsync(trainers, cancellationToken);
                    await _db.SaveChangesAsync(cancellationToken);
                }
                _logger.LogInformation("Seed Workout Trainers Done.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Faild Seeding Workout Trainers.");
            }
        }
    }
}
