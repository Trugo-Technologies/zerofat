using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZeroFat.GymUp.Infrastructure.Persistence.Context;
using ZeroFat.GymUp.Infrastructure.Services;

namespace ZeroFat.GymUp.Infrastructure.Seedings.GymUp;
public class PlansSeeder : IWorkoutSeeder
{
    private readonly GymUpContext _db;
    private readonly ILogger<PlansSeeder> _logger;

    public PlansSeeder(ILogger<PlansSeeder> logger, GymUpContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (!await _db.Plans.AnyAsync(cancellationToken))
        {
            try
            {
                _logger.LogInformation("Started to Seed Workout Plans.");
                string plansData = await File.ReadAllTextAsync(path + "/Seedings/GymUp/14plans.json", cancellationToken);
                var plans = JsonSerializer.Deserialize<List<Plan>>(plansData);

                if (plans != null)
                {
                    foreach (var plan in plans)
                    {
                        plan.IsActive = true;
                        var workouts = await _db.Workouts.ToListAsync(cancellationToken);
                        for (int i = 1; i <= plan.DaysPerWeek; i++)
                        {
                            int index = 1;
                            foreach (var workout in workouts.OrderBy(x => Guid.NewGuid()).Take(new Random().Next(1,3)))
                            {
                                var planSchedule = new PlanSchedule() { WorkoutId = workout.Id, Day = i, Index = index };
                                plan.Schedules.Add(planSchedule);
                                index++;
                            }
                        }
                        plan.PlanGoalId = (await _db.PlanGoals.OrderBy(x => DefaultIdType.NewGuid()).FirstOrDefaultAsync(cancellationToken))?.Id ?? DefaultIdType.Empty;
                        plan.TrainerId = (await _db.Trainers.OrderBy(x => DefaultIdType.NewGuid()).FirstOrDefaultAsync(cancellationToken))?.Id;
                    }

                    await _db.Plans.AddRangeAsync(plans, cancellationToken);
                    await _db.SaveChangesAsync(cancellationToken);
                }
                _logger.LogInformation("Seed Workout Plans Done.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Faild Seeding Workout Trainers.");
            }
        }
    }
}
