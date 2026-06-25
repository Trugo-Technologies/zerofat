using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZeroFat.GymUp.Infrastructure.Persistence.Configurations;

public class BodyPartConfig : IEntityTypeConfiguration<BodyPart>
{
    public void Configure(EntityTypeBuilder<BodyPart> builder)
    {
        builder.ToTable("BodyParts", SchemaNames.GymUp);
    }
}

public class EquipmentConfig : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.ToTable("Equipments", SchemaNames.GymUp);
    }
}

public class EquipmentCategoryConfig : IEntityTypeConfiguration<EquipmentCategory>
{
    public void Configure(EntityTypeBuilder<EquipmentCategory> builder)
    {
        builder.ToTable("EquipmentCategories", SchemaNames.GymUp);
    }
}

public class PlanGoalConfig : IEntityTypeConfiguration<PlanGoal>
{
    public void Configure(EntityTypeBuilder<PlanGoal> builder)
    {
        builder.ToTable("PlanGoals", SchemaNames.GymUp);
    }
}

public class WorkoutTypeConfig : IEntityTypeConfiguration<WorkoutType>
{
    public void Configure(EntityTypeBuilder<WorkoutType> builder)
    {
        builder.ToTable("WorkoutTypes", SchemaNames.GymUp);
    }
}
