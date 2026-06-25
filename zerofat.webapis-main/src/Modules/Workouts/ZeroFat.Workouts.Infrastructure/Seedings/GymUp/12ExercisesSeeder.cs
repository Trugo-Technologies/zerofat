using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZeroFat.GymUp.Infrastructure.Persistence.Context;
using ZeroFat.GymUp.Infrastructure.Services;

namespace ZeroFat.GymUp.Infrastructure.Seedings.GymUp;
public class ExercisesSeeder : IWorkoutSeeder
{
    private readonly GymUpContext _db;
    private readonly ILogger<ExercisesSeeder> _logger;

    public ExercisesSeeder(ILogger<ExercisesSeeder> logger, GymUpContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (!await _db.Exercises.AnyAsync(cancellationToken))
        {
            try
            {
                _logger.LogInformation("Started to Seed Workout exercises.");
                string exercisesData = await File.ReadAllTextAsync(path + "/Seedings/GymUp/12exercises.json", cancellationToken);
                var exercises = JsonSerializer.Deserialize<List<Exercise>>(exercisesData);

                if (exercises != null)
                {
                    foreach (var exercise in exercises)
                    {
                        exercise.IsActive = true;
                        var bodyParts = await _db.BodyParts.OrderBy(x => Guid.NewGuid()).Take(3).Select(x => x.Id).ToListAsync(cancellationToken);
                        exercise.ExerciseBodyParts = bodyParts.ConvertAll(x => new ExerciseBodyPart() { BodyPartId = x });
                        exercise.TrainerId = (await _db.Trainers.OrderBy(x => Guid.NewGuid()).FirstOrDefaultAsync(cancellationToken))?.Id;
                    }

                    await _db.Exercises.AddRangeAsync(exercises, cancellationToken);
                    await _db.SaveChangesAsync(cancellationToken);
                }
                _logger.LogInformation("Seed Workout Exercises Done.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Faild Seeding Workout Trainers.");
            }
        }
    }
}
