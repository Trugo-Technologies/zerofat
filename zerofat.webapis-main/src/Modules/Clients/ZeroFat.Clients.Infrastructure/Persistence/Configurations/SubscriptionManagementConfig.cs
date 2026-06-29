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
        builder.Property(x => x.AddOnItems)
            .HasColumnType("jsonb")
            .HasConversion(new JsonValueConverter<List<SubscriptionWizardAddOnItem>>());
    }
}

public class SubscriptionWizardDraftConfig : IEntityTypeConfiguration<SubscriptionWizardDraft>
{
    public void Configure(EntityTypeBuilder<SubscriptionWizardDraft> builder)
    {
        builder.ToTable("SubscriptionWizardDrafts", SchemaNames.Client);
        builder.Property(x => x.MealTypeSelections)
            .HasColumnType("jsonb")
            .HasConversion(new JsonValueConverter<List<WizardMealTypeSelection>>());
        builder.Property(x => x.AddOnItems)
            .HasColumnType("jsonb")
            .HasConversion(new JsonValueConverter<List<SubscriptionWizardAddOnItem>>());
        builder.Property(x => x.SelectedDeliveryDates)
            .HasColumnType("jsonb")
            .HasConversion(new JsonValueConverter<List<DateOnly>>());
        builder.Property(x => x.SelectedDeliveryDays)
            .HasColumnType("jsonb")
            .HasConversion(new JsonValueConverter<List<DayOfWeek>>());
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

public class ClientAccountActivityLogConfig : IEntityTypeConfiguration<ClientAccountActivityLog>
{
    public void Configure(EntityTypeBuilder<ClientAccountActivityLog> builder)
    {
        builder.ToTable("ClientAccountActivityLogs", SchemaNames.Client);
        builder.HasIndex(x => x.ClientId);
        builder.HasIndex(x => new { x.ClientId, x.CreatedOn });
        builder.Property(x => x.RelatedDate).HasConversion(new DateOnlyConverter(), new DateOnlyComparer());
    }
}

public class MealRatingConfig : IEntityTypeConfiguration<MealRating>
{
    public void Configure(EntityTypeBuilder<MealRating> builder)
    {
        builder.ToTable("MealRatings", SchemaNames.Client);
        builder.HasIndex(x => x.ClientId);
        builder.HasIndex(x => x.DailyMealSelectionId).IsUnique();
        builder.HasIndex(x => x.CreatedOn);
        builder.Property(x => x.MealDate).HasConversion(new DateOnlyConverter(), new DateOnlyComparer());
        builder.Property(x => x.ImprovementTags)
            .HasColumnType("jsonb")
            .HasConversion(new JsonValueConverter<List<MealRatingImprovementTag>>());
    }
}

