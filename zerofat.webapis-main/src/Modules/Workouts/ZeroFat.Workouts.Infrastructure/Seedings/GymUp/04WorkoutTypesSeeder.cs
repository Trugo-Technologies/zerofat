using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZeroFat.GymUp.Infrastructure.Persistence.Context;
using ZeroFat.GymUp.Infrastructure.Services;

namespace ZeroFat.GymUp.Infrastructure.Seedings.GymUp;
public class WorkoutTypesSeeder : IWorkoutSeeder
{
    private readonly GymUpContext _db;
    private readonly ILogger<WorkoutTypesSeeder> _logger;

    public WorkoutTypesSeeder(ILogger<WorkoutTypesSeeder> logger, GymUpContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (!await _db.WorkoutTypes.AnyAsync(cancellationToken))
        {
            try
            {
                _logger.LogInformation("Started to Seed Workout WorkoutTypes.");
                string workoutTypesData = await File.ReadAllTextAsync(path + "/Seedings/GymUp/04workoutTypes.json", cancellationToken);
                var workoutTypes = JsonSerializer.Deserialize<List<WorkoutType>>(workoutTypesData);

                if (workoutTypes != null)
                {
                    foreach (var workoutType in workoutTypes)
                    {
                        workoutType.IsActive = true;
                    }

                    await _db.WorkoutTypes.AddRangeAsync(workoutTypes, cancellationToken);
                    await _db.SaveChangesAsync(cancellationToken);
                }
                _logger.LogInformation("Seed Workout WorkoutTypes Done.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Faild Seeding Workout WorkoutTypes.");
            }
        }
    }
}
