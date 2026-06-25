using ZeroFat.Application.Common.Interfaces;
using ZeroFat.NutriPlan.Application.MealPlanning.Ingredients;
using ZeroFat.NutriPlan.Application.MealPlanning.Meals;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.Extras;

public class ExtraSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? NameEn { get; set; } // Name of the extra (e.g., "Avocado", "Cheese")
    public string? NameAr { get; set; } // Name of the extra (e.g., "Avocado", "Cheese")
    public decimal Price { get; set; } // Additional price for adding this extra
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Amount of the ingredient required for the recipe.
    /// </summary>
    public double Amount { get; set; }
    /// <summary>
    /// Equivalent weight of the ingredient in grams.
    /// </summary>
    public double WeightInGrams { get; set; }

    public DefaultIdType? OrginalIngredientId { get; set; }

    // Navigation properties
    public DefaultIdType? IngredientId { get; set; }
    public DefaultIdType MealId { get; set; }

}

public class ExtraRawDto : ExtraSimplifyDto
{
}

public class ExtraAuditableDto : ExtraRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class ExtraDto : ExtraAuditableDto
{
    public IngredientSimplifyDto? Ingredient { get; set; }
    public MealSimplifyDto Meal { get; set; }
}

public class ExtraDetailsDto : BaseEntityAuditableDetailsDto
{
    public DefaultIdType Id { get; set; }

    public string? NameEn { get; set; } // Name of the extra (e.g., "Avocado", "Cheese")
    public string? NameAr { get; set; } // Name of the extra (e.g., "Avocado", "Cheese")
    public decimal Price { get; set; } // Additional price for adding this extra
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Amount of the ingredient required for the recipe.
    /// </summary>
    public double Amount { get; set; }
    /// <summary>
    /// Equivalent weight of the ingredient in grams.
    /// </summary>
    public double WeightInGrams { get; set; }

    public DefaultIdType? OrginalIngredientId { get; set; }

    // Navigation properties
    public DefaultIdType? IngredientId { get; set; }
    public DefaultIdType MealId { get; set; }

    public IngredientSimplifyDto? Ingredient { get; set; }
    public MealSimplifyDto Meal { get; set; }

}

public class ExtraIngredientMobileDto : IDto
{
    public string? NameEn { get; set; } // Name of the extra (e.g., "Avocado", "Cheese")
    public string? NameAr { get; set; } // Name of the extra (e.g., "Avocado", "Cheese")
    public string? ImageUrl { get; set; } 

    public List<ExtraMobileDto> Extras { get; set; } = [];
    public DefaultIdType Id { get; set; }
}

public class ExtraMobileDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? NameEn { get; set; } // Name of the extra (e.g., "Avocado", "Cheese")
    public string? NameAr { get; set; } // Name of the extra (e.g., "Avocado", "Cheese")
    public decimal Price { get; set; } // Additional price for adding this extra
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Amount of the ingredient required for the recipe.
    /// </summary>
    public double Amount { get; set; }
    /// <summary>
    /// Equivalent weight of the ingredient in grams.
    /// </summary>
    public double WeightInGrams { get; set; }
}
