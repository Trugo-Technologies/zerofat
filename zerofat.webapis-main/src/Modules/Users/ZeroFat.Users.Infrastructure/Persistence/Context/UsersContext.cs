using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Infrastructure.Persistence;
using ZeroFat.Users.Infrastructure.Persistence.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ZeroFat.Infrastructure.Persistence.Configurations;
using ZeroFat.Domain.Enums;

namespace ZeroFat.Users.Infrastructure.Persistence.Context;
public class UsersContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserClaim<string>, ApplicationUserRole, IdentityUserLogin<string>, ApplicationRoleClaim, IdentityUserToken<string>>, IIPDbContext
{
    private readonly ICurrentUser _currentUser;
    private readonly DatabaseOptions _usersModuleOptions;
    public bool EnableAuditing { get; set; }
    public ApplicationModule Module { get; set; } = ApplicationModule.AuditingModule;

    public UsersContext(ICurrentUser currentUser, IOptions<DatabaseOptions> options)
    {
        _currentUser = currentUser;
        _usersModuleOptions = options.Value;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // base.OnConfiguring(optionsBuilder);
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.ConfigureDatabase(_usersModuleOptions.Provider, _usersModuleOptions.ConnectionString);
    }

    public DbSet<Device> Devices { get; set; }
    public DbSet<FailedLoginAttempt> FailedLoginAttempts { get; set; }
    public DbSet<PasswordHistory> PasswordHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationUserConfig).Assembly);
        builder.HasDefaultSchema(IdentityConstants.SchemaName);

    }


    public async Task<int> SaveChangesAsync(bool withAuditing = true, bool withAuditLog = true, bool withDomain = true, CancellationToken cancellationToken = default)
    {
        EnableAuditing = withAuditing;

        int result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return result;
    }
}
