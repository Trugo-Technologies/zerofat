using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ZeroFat.NutriPlan.Infrastructure.Persistence.Configurations;


public class IngredientConfig : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.ToTable("Ingredients", SchemaNames.NutriPlan);
    }
}

public class IngredientAllergenConfig : IEntityTypeConfiguration<IngredientAllergen>
{
    public void Configure(EntityTypeBuilder<IngredientAllergen> builder)
    {
        builder.ToTable("IngredientAllergens", SchemaNames.NutriPlan);
    }
}

public class IngredientAttributeConfig : IEntityTypeConfiguration<IngredientAttribute>
{
    public void Configure(EntityTypeBuilder<IngredientAttribute> builder)
    {
        builder.ToTable("IngredientAttributes", SchemaNames.NutriPlan);
    }
}

public class IngredientMeasurementUnitConfig : IEntityTypeConfiguration<IngredientMeasurementUnit>
{
    public void Configure(EntityTypeBuilder<IngredientMeasurementUnit> builder)
    {
        builder.ToTable("IngredientMeasurementUnits", SchemaNames.NutriPlan);
    }
}

public class RecipeConfig : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.ToTable("Recipes", SchemaNames.NutriPlan);
    }
}

public class RecipeMealTypeConfig : IEntityTypeConfiguration<RecipeMealType>
{
    public void Configure(EntityTypeBuilder<RecipeMealType> builder)
    {
        builder.ToTable("RecipeMealTypes", SchemaNames.NutriPlan);
    }
}

public class RecipeIngredientConfig : IEntityTypeConfiguration<RecipeIngredient>
{
    public void Configure(EntityTypeBuilder<RecipeIngredient> builder)
    {
        builder.ToTable("RecipeIngredients", SchemaNames.NutriPlan);
    }
}

public class MealConfig : IEntityTypeConfiguration<Meal>
{
    public void Configure(EntityTypeBuilder<Meal> builder)
    {
        builder.ToTable("Meals", SchemaNames.NutriPlan);
    }
}

public class MealAllergenConfig : IEntityTypeConfiguration<MealAllergen>
{
    public void Configure(EntityTypeBuilder<MealAllergen> builder)
    {
        builder.ToTable("MealAllergens", SchemaNames.NutriPlan);
    }
}

public class ExtraConfig : IEntityTypeConfiguration<Extra>
{
    public void Configure(EntityTypeBuilder<Extra> builder)
    {
        builder.ToTable("Extras", SchemaNames.NutriPlan);
    }
}

public class MealCustomizationGroupConfig : IEntityTypeConfiguration<MealCustomizationGroup>
{
    public void Configure(EntityTypeBuilder<MealCustomizationGroup> builder)
    {
        builder.ToTable("MealCustomizationGroups", SchemaNames.NutriPlan);
    }
}

public class MealCustomizationOptionConfig : IEntityTypeConfiguration<MealCustomizationOption>
{
    public void Configure(EntityTypeBuilder<MealCustomizationOption> builder)
    {
        builder.ToTable("MealCustomizationOptions", SchemaNames.NutriPlan);
    }
}

public class CustomMealConfig : IEntityTypeConfiguration<CustomMeal>
{
    public void Configure(EntityTypeBuilder<CustomMeal> builder)
    {
        builder.ToTable("CustomMeals", SchemaNames.NutriPlan);
    }
}

public class CustomMealOptionConfig : IEntityTypeConfiguration<CustomMealOption>
{
    public void Configure(EntityTypeBuilder<CustomMealOption> builder)
    {
        builder.ToTable("CustomMealOptions", SchemaNames.NutriPlan);
    }
}
