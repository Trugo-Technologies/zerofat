using ZeroFat.Application.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZeroFat.Infrastructure.Core.Persistence.Context;
using Microsoft.Extensions.DependencyInjection;

namespace ZeroFat.Infrastructure.Core.Persistence.Initialization;
internal sealed class CoreDbInitializer(ILogger<CoreDbInitializer> logger, CoreContext context, IServiceProvider serviceProvider) : IDbInitializer
{
    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        if ((await context.Database.GetPendingMigrationsAsync(cancellationToken).ConfigureAwait(false)).Any())
        {
            await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("applied database migrations for Shared module");
        }
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        // run the database seeder for core models
        var seedRunner = new CoreSeederRunner(serviceProvider);
        await seedRunner.RunSeedersAsync(cancellationToken);
        await Task.Delay(1, cancellationToken);
    }
}
