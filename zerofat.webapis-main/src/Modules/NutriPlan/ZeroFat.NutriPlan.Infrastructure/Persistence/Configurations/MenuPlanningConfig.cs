using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.Bson;
using ZeroFat.NutriPlan.Domain.MenuPlanning;

namespace ZeroFat.NutriPlan.Infrastructure.Persistence.Configurations;

public class MenutConfig : IEntityTypeConfiguration<Menu>
{
    public void Configure(EntityTypeBuilder<Menu> builder)
    {
        builder.ToTable("Menus", SchemaNames.NutriPlan);
    }
}

public class DailyMenuConfig : IEntityTypeConfiguration<DailyMenu>
{
    public void Configure(EntityTypeBuilder<DailyMenu> builder)
    {
        builder.ToTable("DailyMenus", SchemaNames.NutriPlan);
    }
}

public class DailyMenuMealConfig : IEntityTypeConfiguration<DailyMenuMeal>
{
    public void Configure(EntityTypeBuilder<DailyMenuMeal> builder)
    {
        builder.ToTable("DailyMenuMeals", SchemaNames.NutriPlan);
    }
}
