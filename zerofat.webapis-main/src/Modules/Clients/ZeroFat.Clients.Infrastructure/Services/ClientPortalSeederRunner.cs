using Microsoft.Extensions.DependencyInjection;

namespace ZeroFat.ClientPortal.Infrastructure.Services;

internal class ClientPortalSeederRunner
{
    private readonly IClientPortalSeeder[] _seeders;

    public ClientPortalSeederRunner(IServiceProvider serviceProvider) =>
        _seeders = serviceProvider.GetServices<IClientPortalSeeder>().ToArray();

    public async Task RunSeedersAsync(CancellationToken cancellationToken)
    {
        foreach (var seeder in _seeders)
        {
            await seeder.InitializeAsync(cancellationToken);
        }
    }
}
