using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.Bson;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.Infrastructure.Persistence.Values;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.ClientPortal.Infrastructure.Persistence.Configurations;

public class ClientSubscriptionConfig : IEntityTypeConfiguration<ClientSubscription>
{
    public void Configure(EntityTypeBuilder<ClientSubscription> builder)
    {
        builder.ToTable("ClientSubscriptions", SchemaNames.Client);
        //builder.Property(x => x.StartDate).HasConversion(new DateOnlyConverter(), new DateOnlyComparer());
        //builder.Property(x => x.EndDate).HasConversion(new DateOnlyConverter(), new DateOnlyComparer());
    }
}

public class ClientOrderConfig : IEntityTypeConfiguration<ClientOrder>
{
    public void Configure(EntityTypeBuilder<ClientOrder> builder)
    {
        builder.ToTable("ClientOrders", SchemaNames.Client).OwnsMany(
        author => author.ClientOrderItems, ownedNavigationBuilder =>
        {
            ownedNavigationBuilder.ToJson();
        });

        //builder.Property(x => x.StartDate).HasConversion(new DateOnlyConverter(), new DateOnlyComparer());
        //builder.Property(x => x.EndDate).HasConversion(new DateOnlyConverter(), new DateOnlyComparer());
    }
}

public class MealTypeSelectionConfig : IEntityTypeConfiguration<MealTypeSelection>
{
    public void Configure(EntityTypeBuilder<MealTypeSelection> builder)
    {
        builder.ToTable("MealTypeSelections", SchemaNames.Client);
    }
}

public class PaymentConfig : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payment", SchemaNames.Client);
    }
}

public class DailySelectionConfig : IEntityTypeConfiguration<DailySelection>
{
    public void Configure(EntityTypeBuilder<DailySelection> builder)
    {
        builder.ToTable("DailySelections", SchemaNames.Client);
    }
}

public class DailyMealSelectionConfig : IEntityTypeConfiguration<DailyMealSelection>
{
    public void Configure(EntityTypeBuilder<DailyMealSelection> builder)
    {
        builder.ToTable("DailyMealSelections", SchemaNames.Client);
    }
}

