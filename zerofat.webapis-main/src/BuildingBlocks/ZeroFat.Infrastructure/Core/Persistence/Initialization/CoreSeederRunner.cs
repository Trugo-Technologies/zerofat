using ZeroFat.Infrastructure.Core.Services;

namespace ZeroFat.Infrastructure.Core.Persistence.Initialization;

internal class CoreSeederRunner
{
    private readonly ICoreSeeder[] _seeders;

    public CoreSeederRunner(IServiceProvider serviceProvider) =>
        _seeders = serviceProvider.GetServices<ICoreSeeder>().ToArray();

    public async Task RunSeedersAsync(CancellationToken cancellationToken)
    {
        foreach (var seeder in _seeders)
        {
            await seeder.InitializeAsync(cancellationToken);
        }
    }
}
