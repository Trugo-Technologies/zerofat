using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZeroFat.ClientPortal.Domain.Discounts;

namespace ZeroFat.ClientPortal.Infrastructure.Persistence.Configurations;

public class DiscountRuleConfig : IEntityTypeConfiguration<DiscountRule>
{
    public void Configure(EntityTypeBuilder<DiscountRule> builder)
    {
        builder.ToTable("DiscountRules", SchemaNames.Client);
    }
}

public class DiscountRedemptionConfig : IEntityTypeConfiguration<DiscountRedemption>
{
    public void Configure(EntityTypeBuilder<DiscountRedemption> builder)
    {
        builder.ToTable("DiscountRedemptions", SchemaNames.Client);
    }
}
