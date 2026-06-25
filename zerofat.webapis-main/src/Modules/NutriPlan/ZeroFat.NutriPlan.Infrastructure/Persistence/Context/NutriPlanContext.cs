using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ZeroFat.Domain.Common.Contracts;
using MediatR;
using ZeroFat.NutriPlan.Domain.Settings;
using ZeroFat.Infrastructure.Persistence.Configurations;
using ZeroFat.NutriPlan.Domain.MealPlanning;
using ZeroFat.NutriPlan.Domain.MenuPlanning;

namespace ZeroFat.NutriPlan.Infrastructure.Persistence.Context;

public class NutriPlanContext : IPDbContext
{
    public NutriPlanContext(IPublisher publisher, DbContextOptions<NutriPlanContext> options, ICurrentUser currentUser, IOptions<DatabaseOptions> settings) : base(publisher, options, currentUser, settings)
    {
    }


    #region Catalog 

    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<IngredientAllergen> IngredientAllergens => Set<IngredientAllergen>();
    public DbSet<IngredientAttribute> IngredientAttributes => Set<IngredientAttribute>();
    public DbSet<IngredientMeasurementUnit> IngredientMeasurementUnits => Set<IngredientMeasurementUnit>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<RecipeMealType> RecipeMealTypes => Set<RecipeMealType>();
    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();
    public DbSet<Meal> Meals => Set<Meal>();
    public DbSet<Extra> Extras => Set<Extra>();
    public DbSet<MealCustomizationGroup> MealCustomizationGroups => Set<MealCustomizationGroup>();
    public DbSet<MealCustomizationOption> MealCustomizationOptions => Set<MealCustomizationOption>();
    #endregion

    #region Settings 
    public DbSet<Allergen> Allergens => Set<Allergen>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<MealType> MealTypes => Set<MealType>();
    public DbSet<MealPlan> MealPlans => Set<MealPlan>();
    public DbSet<MealPlanMealType> MealPlanMealTypes => Set<MealPlanMealType>();
    public DbSet<MeasurementUnit> MeasurementUnits => Set<MeasurementUnit>();
    public DbSet<NutrientsAttribute> NutrientsAttributes => Set<NutrientsAttribute>();
    #endregion

    public DbSet<Menu> Menus => Set<Menu>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.AppendGlobalQueryFilter<ISoftDelete>(s => s.DeletedOn == null);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
