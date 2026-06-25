using Microsoft.Extensions.DependencyInjection;

namespace ZeroFat.GymUp.Infrastructure.Services;

internal class WorkoutSeederRunner
{
    private static readonly string[] SeederOrder =
    [
        "BodyPartsSeeder",
        "EquipmentCategoriesSeeder",
        "PlanGoalsSeeder",
        "WorkoutTypesSeeder",
        "TrainersSeeder",
        "ExercisesSeeder",
        "WorkoutsSeeder",
        "PlansSeeder",
    ];

    private readonly IWorkoutSeeder[] _seeders;

    public WorkoutSeederRunner(IServiceProvider serviceProvider)
    {
        _seeders = serviceProvider.GetServices<IWorkoutSeeder>()
            .OrderBy(s => Array.IndexOf(SeederOrder, s.GetType().Name))
            .ToArray();
    }

    public async Task RunSeedersAsync(CancellationToken cancellationToken)
    {
        foreach (var seeder in _seeders)
        {
            await seeder.InitializeAsync(cancellationToken);
        }
    }
}
