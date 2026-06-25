using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ZeroFat.Domain.Common.Contracts;
using MediatR;
using ZeroFat.Infrastructure.Persistence.Configurations;
using ZeroFat.NutriPlan.Domain.Settings;
using ZeroFat.NutriPlan.Domain.MenuPlanning;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.NutriPlan.Domain.MealPlanning;
using ZeroFat.ClientPortal.Domain.NutritionTracking;
using ZeroFat.ClientPortal.Domain.Discounts;

namespace ZeroFat.ClientPortal.Infrastructure.Persistence.Context;
public class ClientPortalContext : IPDbContext
{
    #region Client 
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<ClientLocation> ClientLocations => Set<ClientLocation>();
    public DbSet<ClientGoal> ClientGoals => Set<ClientGoal>();
    public DbSet<ClientChat> ClientChats => Set<ClientChat>();
    public DbSet<ClientPaymentMethod> ClientPaymentMethods => Set<ClientPaymentMethod>();
    public DbSet<ClientLoyaltyPoint> ClientLoyaltyPoints => Set<ClientLoyaltyPoint>();
    #endregion

    public DbSet<ClientSubscription> ClientSubscriptions => Set<ClientSubscription>();
    public DbSet<SubscriptionWizardDraft> SubscriptionWizardDrafts => Set<SubscriptionWizardDraft>();
    public DbSet<ClientOrder> ClientOrders => Set<ClientOrder>();
    public DbSet<Payment> Payments => Set<Payment>();

    public DbSet<DailySelection> DailySelections => Set<DailySelection>();
    public DbSet<DailyMealSelection> DailyMealSelections => Set<DailyMealSelection>();


    public DbSet<DiscountRule> DiscountRules => Set<DiscountRule>();
    public DbSet<DiscountRedemption> DiscountRedemptions => Set<DiscountRedemption>();

    #region Nutration Tracking 
    public DbSet<CalorieRecord> CalorieRecords => Set<CalorieRecord>();
    public DbSet<DailyHealthLog> DailyHealthLogs => Set<DailyHealthLog>();
    #endregion




    public ClientPortalContext(IPublisher publisher, DbContextOptions<ClientPortalContext> options, ICurrentUser currentUser, IOptions<DatabaseOptions> settings) : base(publisher, options, currentUser, settings)
    {
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {

        base.OnModelCreating(builder);

        builder.AppendGlobalQueryFilter<ISoftDelete>(s => s.DeletedOn == null);

        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);

        builder.Entity<MealType>().ToTable("MealTypes", schema: "NutriPlan", x => x.ExcludeFromMigrations());
        builder.Entity<MealPlan>().ToTable("MealPlans", schema: "NutriPlan", x => x.ExcludeFromMigrations());
        builder.Entity<DailyMenuMeal>().ToTable("DailyMenuMeals", schema: "NutriPlan", x => x.ExcludeFromMigrations());
        builder.Entity<Meal>().ToTable("Meals", schema: "NutriPlan", x => x.ExcludeFromMigrations());
        builder.Entity<Recipe>().ToTable("Recipes", schema: "NutriPlan", x => x.ExcludeFromMigrations());
        builder.Entity<Allergen>().ToTable("Allergens", schema: "NutriPlan", x => x.ExcludeFromMigrations());
        builder.Entity<MealAllergen>().ToTable("MealAllergens", schema: "NutriPlan", x => x.ExcludeFromMigrations());
        builder.Entity<MealPlanMealType>().ToTable("MealPlanMealTypes", schema: "NutriPlan", x => x.ExcludeFromMigrations());
        builder.Entity<Menu>().ToTable("Menus", schema: "NutriPlan", x => x.ExcludeFromMigrations());
        builder.Entity<DailyMenu>().ToTable("DailyMenus", schema: "NutriPlan", x => x.ExcludeFromMigrations());
        builder.Entity<Category>().ToTable("Categories", schema: "NutriPlan", x => x.ExcludeFromMigrations());
        builder.Entity<MeasurementUnit>().ToTable("MeasurementUnits", schema: "NutriPlan", x => x.ExcludeFromMigrations());
        builder.Entity<NutrientsAttribute>().ToTable("NutrientsAttributes", schema: "NutriPlan", x => x.ExcludeFromMigrations());
        builder.Entity<RecipeMealType>().ToTable("RecipeMealTypes", schema: "NutriPlan", x => x.ExcludeFromMigrations());
        builder.Entity<Ingredient>().ToTable("Ingredients", schema: "NutriPlan", x => x.ExcludeFromMigrations());
        builder.Entity<Extra>().ToTable("Extras", schema: "NutriPlan", x => x.ExcludeFromMigrations());
        builder.Entity<IngredientAllergen>().ToTable("IngredientAllergens", schema: "NutriPlan", x => x.ExcludeFromMigrations());
        builder.Entity<IngredientAttribute>().ToTable("IngredientAttributes", schema: "NutriPlan", x => x.ExcludeFromMigrations());
        builder.Entity<IngredientMeasurementUnit>().ToTable("IngredientMeasurementUnits", schema: "NutriPlan", x => x.ExcludeFromMigrations());
        builder.Entity<RecipeIngredient>().ToTable("RecipeIngredients", schema: "NutriPlan", x => x.ExcludeFromMigrations());
    }



}
