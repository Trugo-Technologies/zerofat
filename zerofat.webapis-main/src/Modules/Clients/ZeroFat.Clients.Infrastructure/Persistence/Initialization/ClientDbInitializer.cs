using ZeroFat.Application.Common.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ZeroFat.ClientPortal.Infrastructure.Persistence.Context;
using Microsoft.Extensions.DependencyInjection;
using ZeroFat.ClientPortal.Infrastructure.Services;

namespace ZeroFat.ClientPortal.Infrastructure.Persistence.Initialization;
internal sealed class ClientDbInitializer(ILogger<ClientDbInitializer> logger, ClientPortalContext context, IServiceProvider serviceProvider) : IDbInitializer
{
    public async Task MigrateAsync(CancellationToken cancellationToken)
    {
        if ((await context.Database.GetPendingMigrationsAsync(cancellationToken).ConfigureAwait(false)).Any())
        {
            await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("applied database migrations for client module");
        }
    }

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        // run the database seeder for core models
        var seedRunner = new ClientPortalSeederRunner(serviceProvider);
        await seedRunner.RunSeedersAsync(cancellationToken);
        await Task.Delay(1, cancellationToken);
    }


}
