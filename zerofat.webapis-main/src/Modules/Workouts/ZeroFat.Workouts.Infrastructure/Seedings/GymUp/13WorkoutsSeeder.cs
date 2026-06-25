using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZeroFat.GymUp.Infrastructure.Persistence.Context;
using ZeroFat.GymUp.Infrastructure.Services;

namespace ZeroFat.GymUp.Infrastructure.Seedings.GymUp;
public class WorkoutsSeeder : IWorkoutSeeder
{
    private readonly GymUpContext _db;
    private readonly ILogger<WorkoutsSeeder> _logger;

    public WorkoutsSeeder(ILogger<WorkoutsSeeder> logger, GymUpContext db)
    {
        _logger = logger;
        _db = db;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        string? path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (!await _db.Workouts.AnyAsync(cancellationToken))
        {
            try
            {
                _logger.LogInformation("Started to Seed Workout Workouts.");
                string workoutsData = await File.ReadAllTextAsync(path + "/Seedings/GymUp/13workouts.json", cancellationToken);
                var workouts = JsonSerializer.Deserialize<List<Workout>>(workoutsData);

                if (workouts != null)
                {
                    foreach (var workout in workouts)
                    {
                        workout.IsActive = true;
                        var bodyParts = await _db.BodyParts.OrderBy(x => Guid.NewGuid()).Take(3).Select(x => x.Id).ToListAsync(cancellationToken);
                        var equipments = await _db.Equipments.OrderBy(x => Guid.NewGuid()).Take(3).Select(x => x.Id).ToListAsync(cancellationToken);
                        workout.WorkoutBodyParts = bodyParts.ConvertAll(x => new WorkoutBodyPart() { BodyPartId = x });
                        workout.WorkoutEquipments = equipments.ConvertAll(x => new WorkoutEquipment() { EquipmentId = x });
                        workout.SetNamesAr = ["Set Name1", "Other Set"];
                        workout.SetNamesEn = ["Set Name1", "Other Set"];
                        if (workout.Format != ZeroFat.Domain.Enums.WorkoutFormat.FollowAlong)
                        {
                            var exercises = await _db.Exercises.OrderBy(x => Guid.NewGuid()).Take(5).ToListAsync(cancellationToken);
                            int index = 1;
                            int setIndex = 1;
                            foreach (var exercise in exercises)
                            {
                                var workoutExercise = new WorkoutExercise() { ExerciseId = exercise.Id, SetIndex = setIndex, Index = index };
                                if(exercise.Type == ZeroFat.Domain.Enums.ExerciseType.Duration)
                                {
                                    workoutExercise.DurationInSec = 40;
                                }
                                if (exercise.Type == ZeroFat.Domain.Enums.ExerciseType.Reps)
                                {
                                    workoutExercise.Reps = 15;
                                }
                                if (exercise.Type == ZeroFat.Domain.Enums.ExerciseType.WeightAndReps)
                                {
                                    workoutExercise.Reps = 12;
                                    workoutExercise.Weight = 30;
                                }
                                workout.WorkoutExercises.Add(workoutExercise);
                                index++;
                                if (index > 3)
                                {
                                    index = 1;
                                    setIndex++;
                                }
                            }
                        }

                        workout.TrainerId = (await _db.Trainers.OrderBy(x => Guid.NewGuid()).FirstOrDefaultAsync(cancellationToken))?.Id;
                        workout.WorkoutTypeId = (await _db.WorkoutTypes.OrderBy(x => Guid.NewGuid()).FirstOrDefaultAsync(cancellationToken))?.Id ?? Guid.Empty;
                    }

                    await _db.Workouts.AddRangeAsync(workouts, cancellationToken);
                    await _db.SaveChangesAsync(cancellationToken);
                }
                _logger.LogInformation("Seed Workout Workouts Done.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Faild Seeding Workout Trainers.");
            }
        }
    }
}
