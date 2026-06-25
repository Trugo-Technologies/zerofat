using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ZeroFat.Infrastructure.Persistence.Configurations;
using ZeroFat.Domain.Enums;

namespace ZeroFat.Infrastructure.Persistence;
public class IPDbContext : DbContext, IIPDbContext
{
    private readonly DatabaseOptions? _settings;
    private readonly ICurrentUser _currentUser;
    private readonly IPublisher _publisher;

    public IPDbContext(IPublisher publisher, DbContextOptions options, ICurrentUser currentUser, IOptions<DatabaseOptions> settings) : base(options)
    {
        _settings = settings.Value;
        _currentUser = currentUser;
        _publisher = publisher;
    }

    public IPDbContext(IPublisher publisher, DbContextOptions options, ICurrentUser currentUser, IOptions<ZeroFatSeperateModule> settings) : base(options)
    {
        _settings = settings.Value?.DatabaseOptions;
        _currentUser = currentUser;
        _publisher = publisher;
    }

    public bool EnableAuditing { get; set; }
    public ApplicationModule Module { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
        if(_settings != null)
        {
            optionsBuilder.ConfigureDatabase(_settings.Provider, _settings.ConnectionString, _settings.DatabaseName);
        }
    }

    public async Task<int> SaveChangesAsync(bool withAuditing = true, bool withAuditLog = true, bool withDomain = true, CancellationToken cancellationToken = default)
    {
        EnableAuditing = withAuditing;

        int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return result;
    }
}
