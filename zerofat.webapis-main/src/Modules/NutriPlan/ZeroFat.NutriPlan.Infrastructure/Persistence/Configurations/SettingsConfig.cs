using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.Bson;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Infrastructure.Persistence.Configurations;

public class MeasurementUnitConfig : IEntityTypeConfiguration<MeasurementUnit>
{
    public void Configure(EntityTypeBuilder<MeasurementUnit> builder)
    {
        builder.ToTable("MeasurementUnits", SchemaNames.NutriPlan);
    }
}

public class NutrientsAttributeConfig : IEntityTypeConfiguration<NutrientsAttribute>
{
    public void Configure(EntityTypeBuilder<NutrientsAttribute> builder)
    {
        builder.ToTable("NutrientsAttributes", SchemaNames.NutriPlan);
    }
}

public class AllergenConfig : IEntityTypeConfiguration<Allergen>
{
    public void Configure(EntityTypeBuilder<Allergen> builder)
    {
        builder.ToTable("Allergens", SchemaNames.NutriPlan);
    }
}

public class CategoryConfig : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories", SchemaNames.NutriPlan);
    }
}

public class MealTypeConfig : IEntityTypeConfiguration<MealType>
{
    public void Configure(EntityTypeBuilder<MealType> builder)
    {
        builder.ToTable("MealTypes", SchemaNames.NutriPlan);
    }
}

public class MealPlanConfig : IEntityTypeConfiguration<MealPlan>
{
    public void Configure(EntityTypeBuilder<MealPlan> builder)
    {
        builder.ToTable("MealPlans", SchemaNames.NutriPlan);
    }
}

public class MealPlanMealTypeConfig : IEntityTypeConfiguration<MealPlanMealType>
{
    public void Configure(EntityTypeBuilder<MealPlanMealType> builder)
    {
        builder.ToTable("MealPlanMealTypes", SchemaNames.NutriPlan);
    }
}
