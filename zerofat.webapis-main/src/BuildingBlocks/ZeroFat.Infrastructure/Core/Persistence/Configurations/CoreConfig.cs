using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZeroFat.Domain.Core;
using ZeroFat.Domain.Core.Common;

namespace ZeroFat.Infrastructure.Core.Persistence.Configurations;

public class FaqCategoryConfig : IEntityTypeConfiguration<FaqCategory>
{
    public void Configure(EntityTypeBuilder<FaqCategory> builder)
    {
        builder.ToTable(CoreResource.FAQCategories, SchemaNames.Core);
    }
}

public class FaqConfig : IEntityTypeConfiguration<Faq>
{
    public void Configure(EntityTypeBuilder<Faq> builder)
    {
        builder.ToTable(CoreResource.FAQs, SchemaNames.Core);
    }
}


public class PhysicalActivityLevelConfig : IEntityTypeConfiguration<PhysicalActivityLevel>
{
    public void Configure(EntityTypeBuilder<PhysicalActivityLevel> builder)
    {
        builder.ToTable(CoreResource.PhysicalActivityLevels, SchemaNames.Core);
    }
}

public class SettingConfig : IEntityTypeConfiguration<Setting>
{
    public void Configure(EntityTypeBuilder<Setting> builder)
    {
        builder.ToTable(CoreResource.Settings, SchemaNames.Core);
    }
}

public class SubscriptionPlanConfig : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.ToTable(CoreResource.SubscriptionPlans, SchemaNames.Core);
    }
}

public class AdvertisementsConfig : IEntityTypeConfiguration<Advertisement>
{
    public void Configure(EntityTypeBuilder<Advertisement> builder)
    {
        builder.ToTable(CoreResource.Advertisements, SchemaNames.Core);
    }
}

public class BannersConfig : IEntityTypeConfiguration<Banner>
{
    public void Configure(EntityTypeBuilder<Banner> builder)
    {
        builder.ToTable(CoreResource.Banners, SchemaNames.Core);
    }
}
