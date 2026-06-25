namespace ZeroFat.NutriPlan.Application.Statistics;

public class StatisticsDto
{
    public int MealPlans { get; set; }
    public int Menus { get; set; }
    public int PublishedMenus { get; set; }
    public int Recipes { get; set; }
    public int ColdRecipes { get; set; }
    public int WarmRecipes { get; set; }
    public int Meals { get; set; }
    public int Ingredients { get; set; }
    public int Extras { get; set; }
    public Dictionary<string, int> MealByCuisineType { get; set; } = new Dictionary<string, int>();
    public Dictionary<string, int> IngredientsByStatus { get; set; } = new Dictionary<string, int>();
    public Dictionary<string, int> IngredientsByType { get; set; } = new Dictionary<string, int>();
    public Dictionary<string, int> RecipeByDifficulty { get; set; } = new Dictionary<string, int>();
}
