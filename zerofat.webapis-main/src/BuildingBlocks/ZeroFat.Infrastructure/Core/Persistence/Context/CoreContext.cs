using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ZeroFat.Domain.Common.Contracts;
using MediatR;
using ZeroFat.Domain.Core;
using ZeroFat.Infrastructure.Persistence.Configurations;

namespace ZeroFat.Infrastructure.Core.Persistence.Context;
public class CoreContext : IPDbContext
{
    public CoreContext(IPublisher publisher, DbContextOptions<CoreContext> options, ICurrentUser currentUser, IOptions<DatabaseOptions> settings) : base(publisher, options, currentUser, settings)
    {
    }

    #region Advertisement
    public DbSet<Advertisement> Advertisements => Set<Advertisement>();
    public DbSet<Faq> Faqs => Set<Faq>();
    public DbSet<FaqCategory> FaqCategories => Set<FaqCategory>();
    public DbSet<Setting> Settings => Set<Setting>();
    public DbSet<PhysicalActivityLevel> PhysicalActivityLevels => Set<PhysicalActivityLevel>();
    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
    #endregion

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.AppendGlobalQueryFilter<ISoftDelete>(s => s.DeletedOn == null);
        builder.ApplyEntityConfigurations(GetType().Assembly, "ZeroFat.Infrastructure.Core.Persistence.Configurations");
    }
}
