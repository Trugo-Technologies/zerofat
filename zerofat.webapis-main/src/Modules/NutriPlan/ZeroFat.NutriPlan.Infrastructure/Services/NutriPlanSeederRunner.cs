using Microsoft.Extensions.DependencyInjection;

namespace ZeroFat.NutriPlan.Infrastructure.Services;

internal class NutriPlanSeederRunner
{
    private static readonly string[] SeederOrder =
    [
        "AllergensSeeder",
        "CategoriesSeeder",
        "MeasurementUnitsSeeder",
        "MealPlansSeeder",
        "NutriPlanSettingsSeeder",
        "RecipesSeeder",
    ];

    private readonly INutriPlanSeeder[] _seeders;

    public NutriPlanSeederRunner(IServiceProvider serviceProvider) =>
        _seeders = serviceProvider.GetServices<INutriPlanSeeder>()
            .OrderBy(s => Array.IndexOf(SeederOrder, s.GetType().Name))
            .ToArray();

    public async Task RunSeedersAsync(CancellationToken cancellationToken)
    {
        foreach (var seeder in _seeders)
        {
            await seeder.InitializeAsync(cancellationToken);
        }
    }
}
