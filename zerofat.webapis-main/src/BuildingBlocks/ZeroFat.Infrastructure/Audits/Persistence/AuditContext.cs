using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Audits;
using ZeroFat.Domain.Enums;
using ZeroFat.Infrastructure.Persistence;

namespace ZeroFat.Infrastructure.Audits.Persistence;

public class AuditContext : IPDbContext
{
    public DbSet<AuditTrail> AuditTrails { get; set; }

    public AuditContext(IPublisher publisher, DbContextOptions<AuditContext> options, ICurrentUser currentUser, IOptions<AuditModuleOptions> settings) : base(publisher, options, currentUser, settings)
    {
        EnableAuditing = false;
        Module = ApplicationModule.AuditingModule;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure the enum to be stored as a string
        modelBuilder.Entity<AuditTrail>()
            .Property(u => u.Module)
            .HasConversion(
                v => v.ToString(), // From Enum to String for storage
                v => (ApplicationModule)Enum.Parse(typeof(ApplicationModule), v) // From String to Enum when reading
            );
    }
}
