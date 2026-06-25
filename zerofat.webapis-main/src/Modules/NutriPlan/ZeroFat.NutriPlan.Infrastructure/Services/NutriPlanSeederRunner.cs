using Microsoft.Extensions.DependencyInjection;

namespace ZeroFat.NutriPlan.Infrastructure.Services;

internal class NutriPlanSeederRunner
{
    private readonly INutriPlanSeeder[] _seeders;

    public NutriPlanSeederRunner(IServiceProvider serviceProvider) =>
        _seeders = serviceProvider.GetServices<INutriPlanSeeder>().ToArray();

    public async Task RunSeedersAsync(CancellationToken cancellationToken)
    {
        foreach (var seeder in _seeders)
        {
            await seeder.InitializeAsync(cancellationToken);
        }
    }
}
