using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.Bson;
using ZeroFat.ClientPortal.Domain.NutritionTracking;
using ZeroFat.Domain.Enums;
using ZeroFat.Infrastructure.Persistence.Values;

namespace ZeroFat.ClientPortal.Infrastructure.Persistence.Configurations;

public class ClientConfig : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Clients", SchemaNames.Client);
    }
}

public class ClientLocationConfig : IEntityTypeConfiguration<ClientLocation>
{
    public void Configure(EntityTypeBuilder<ClientLocation> builder)
    {
        builder.ToTable("ClientLocations", SchemaNames.Client);
    }
}

public class ClientGoalConfig : IEntityTypeConfiguration<ClientGoal>
{
    public void Configure(EntityTypeBuilder<ClientGoal> builder)
    {
        builder.ToTable("ClientGoals", SchemaNames.Client);
    }
}

//public class ClientDailyStatisticsConfig : IEntityTypeConfiguration<ClientDailyStatistics>
//{
//    public void Configure(EntityTypeBuilder<ClientDailyStatistics> builder)
//    {
//        builder.ToTable("ClientDailyStatistics", SchemaNames.Client);

//        builder.OwnsMany(
//        author => author.CaloriesDailyStatistics, ownedNavigationBuilder =>
//        {
//            ownedNavigationBuilder.ToJson();
//        });

//        builder.Property(x => x.Date).HasConversion(new DateOnlyConverter(), new DateOnlyComparer());
//    }
//}

public class ClientChatsConfig : IEntityTypeConfiguration<ClientChat>
{
    public void Configure(EntityTypeBuilder<ClientChat> builder)
    {
        builder.ToTable("ClientChats", SchemaNames.Client);
    }
}

public class ClientLoyaltyPointsConfig : IEntityTypeConfiguration<ClientLoyaltyPoint>
{
    public void Configure(EntityTypeBuilder<ClientLoyaltyPoint> builder)
    {
        builder.ToTable("ClientLoyaltyPoints", SchemaNames.Client);
    }
}

public class ClientPaymentMethodsConfig : IEntityTypeConfiguration<ClientPaymentMethod>
{
    public void Configure(EntityTypeBuilder<ClientPaymentMethod> builder)
    {
        builder.ToTable("ClientPaymentMethodS", SchemaNames.Client);
    }
}


public class DailyHealthLogConfig : IEntityTypeConfiguration<DailyHealthLog>
{
    public void Configure(EntityTypeBuilder<DailyHealthLog> builder)
    {
        builder.ToTable("DailyHealthLogs", SchemaNames.Client);

        // Value conversions for value objects
        builder.OwnsOne(x => x.Weight, weightBuilder =>
        {
            // Configure the Value property
            weightBuilder.Property(p => p.Value)
                .HasColumnName("WeightValue");

            // Configure the Unit as a separate owned entity
            weightBuilder.OwnsOne(w => w.Unit, unitBuilder =>
            {
                unitBuilder.Property(u => u.Symbol)
                    .HasColumnName("WeightUnit")
                    .HasMaxLength(10);
            });
        });

        builder.OwnsOne(x => x.WaterConsumption, w =>
        {
            w.Property(p => p.Liters).HasColumnName("WaterIntakeLiters");
        });

        //// Configure computed columns
        //builder.Property(x => x.TotalCaloriesConsumed)
        //    .HasColumnName("TotalCaloriesConsumed")
        //    .HasColumnType("decimal(10,2)")
        //    .HasComputedColumnSql(
        //        @"(SELECT ISNULL(SUM([Calories]), 0) 
        //          FROM [Client].[CalorieRecords] cr 
        //          WHERE cr.DailyHealthLogId = [Id] 
        //          AND cr.RecordType = 'Food')",
        //        stored: true);

        //builder.Property(x => x.TotalCaloriesBurned)
        //    .HasColumnName("TotalCaloriesBurned")
        //    .HasColumnType("decimal(10,2)")
        //    .HasComputedColumnSql(
        //        @"(SELECT ISNULL(SUM([Calories]), 0) 
        //          FROM [Client].[CalorieRecords] cr 
        //          WHERE cr.DailyHealthLogId = [Id] 
        //          AND cr.RecordType = 'Activity')",
        //        stored: true);

        builder.Ignore(x => x.NetCalories); // Still calculated in memory

        // Relationships
        builder.HasOne(x => x.Client)
            .WithMany()
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.CalorieRecords)
            .WithOne()
            .HasForeignKey(x => x.DailyHealthLogId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(x => new { x.ClientId, x.LogDate })
            .IsUnique()
            .HasDatabaseName("IX_DailyHealthLog_ClientDate");

        builder.HasIndex(x => x.LogDate)
            .HasDatabaseName("IX_DailyHealthLog_LogDate");
    }
}


public class CalorieRecordConfig : IEntityTypeConfiguration<CalorieRecord>
{
    public void Configure(EntityTypeBuilder<CalorieRecord> builder)
    {
        builder.ToTable("CalorieRecords", SchemaNames.Client);

        // Primary key
        builder.HasKey(x => x.Id);

        // Value conversions
        builder.Property(x => x.RecordType)
            .HasConversion(
                v => v.ToString(),
                v => (CalorieRecordType)Enum.Parse(typeof(CalorieRecordType), v))
            .HasMaxLength(10);

        builder.OwnsOne(x => x.Nutrition, n =>
        {
            n.Property(p => p.Calories).HasColumnName("Calories");
            n.Property(p => p.ProteinInGrams).HasColumnName("ProteinInGrams");
            n.Property(p => p.CarbsInGrams).HasColumnName("CarbsInGrams");
            n.Property(p => p.FatInGrams).HasColumnName("FatInGrams");
        });

        // Properties
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.RecordedAt)
            .IsRequired()
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.Property(x => x.MealTime)
            .HasColumnType("time(0)");

        // Indexes
        builder.HasIndex(x => x.DailyHealthLogId)
            .HasDatabaseName("IX_CalorieRecords_DailyHealthLogId");

        builder.HasIndex(x => x.RecordType)
            .HasDatabaseName("IX_CalorieRecords_RecordType");
    }
}
