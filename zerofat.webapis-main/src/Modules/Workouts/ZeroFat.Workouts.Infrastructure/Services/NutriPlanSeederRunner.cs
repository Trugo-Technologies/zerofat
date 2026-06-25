using Microsoft.Extensions.DependencyInjection;

namespace ZeroFat.GymUp.Infrastructure.Services;

internal class WorkoutSeederRunner
{
    private readonly IWorkoutSeeder[] _seeders;

    public WorkoutSeederRunner(IServiceProvider serviceProvider)
    {
        _seeders = serviceProvider.GetServices<IWorkoutSeeder>().ToArray();
    }

    public async Task RunSeedersAsync(CancellationToken cancellationToken)
    {
        foreach (var seeder in _seeders)
        {
            await seeder.InitializeAsync(cancellationToken);
        }
    }
}
