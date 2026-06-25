using ZeroFat.Application.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZeroFat.GymUp.Infrastructure.Persistence.Context;
using ZeroFat.GymUp.Infrastructure.Services;

namespace ZeroFat.GymUp.Infrastructure.Persistence.Initialization;
internal sealed class WorkoutDbInitializer(ILogger<WorkoutDbInitializer> logger, GymUpContext context, IServiceProvider serviceProvider) : IDbInitializer
{
    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        if ((await context.Database.GetPendingMigrationsAsync(cancellationToken).ConfigureAwait(false)).Any())
        {
            await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("applied database migrations for Workout module");
        }
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        // run the database seeder for core models
        var seedRunner = new WorkoutSeederRunner(serviceProvider);
        await seedRunner.RunSeedersAsync(cancellationToken);
        await Task.Delay(1, cancellationToken);
    }

}
