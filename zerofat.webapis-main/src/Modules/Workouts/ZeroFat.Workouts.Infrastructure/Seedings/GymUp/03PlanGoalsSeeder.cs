using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZeroFat.GymUp.Infrastructure.Persistence.Context;
using ZeroFat.GymUp.Infrastructure.Services;

namespace ZeroFat.GymUp.Infrastructure.Seedings.GymUp;
public class PlanGoalsSeeder : IWorkoutSeeder
{
    private readonly GymUpContext _db;
    private readonly ILogger<PlanGoalsSeeder> _logger;

    public PlanGoalsSeeder(ILogger<PlanGoalsSeeder> logger, GymUpContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (!await _db.PlanGoals.AnyAsync(cancellationToken))
        {
            try
            {
                _logger.LogInformation("Started to Seed Workout PlanGoals.");
                string planGoalsData = await File.ReadAllTextAsync(path + "/Seedings/GymUp/03planGoals.json", cancellationToken);
                var planGoals = JsonSerializer.Deserialize<List<PlanGoal>>(planGoalsData);

                if (planGoals != null)
                {
                    foreach (var planGoal in planGoals)
                    {
                        planGoal.IsActive = true;
                    }

                    await _db.PlanGoals.AddRangeAsync(planGoals, cancellationToken);
                    await _db.SaveChangesAsync(cancellationToken);
                }
                _logger.LogInformation("Seed Workout PlanGoals Done.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Faild Seeding Workout PlanGoals.");
            }
        }
    }
}
