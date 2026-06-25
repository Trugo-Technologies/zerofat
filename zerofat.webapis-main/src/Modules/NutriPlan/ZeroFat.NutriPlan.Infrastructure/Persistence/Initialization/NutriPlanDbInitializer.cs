using ZeroFat.Application.Common.Persistence;
using ZeroFat.NutriPlan.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using ZeroFat.NutriPlan.Infrastructure.Services;

namespace ZeroFat.NutriPlan.Infrastructure.Persistence.Initialization;
internal sealed class NutriPlanDbInitializer(
    ILogger<NutriPlanDbInitializer> logger, 
    IServiceProvider serviceProvider,
    NutriPlanContext context) : IDbInitializer
{
    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        if ((await context.Database.GetPendingMigrationsAsync(cancellationToken).ConfigureAwait(false)).Any())
        {
            await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("applied database migrations for NutriPlan module");
        }
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        // run the database seeder for core models
        var seedRunner = new NutriPlanSeederRunner(serviceProvider);
        await seedRunner.RunSeedersAsync(cancellationToken);
        await Task.Delay(1, cancellationToken);
    }

}
