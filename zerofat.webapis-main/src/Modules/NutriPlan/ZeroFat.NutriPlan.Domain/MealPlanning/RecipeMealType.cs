using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Domain.MealPlanning;

public class RecipeMealType : Entity, IAggregateRoot
{
    public DefaultIdType? MealTypeId { get; set; }
    public MealType? MealType { get; set; }
    public DefaultIdType? RecipeId { get; set; }
    public Recipe? Recipe { get; set; }
}


//public class Meal : ActivationEntity, IAggregateRoot
//{
//    public string? NameEn { get; set; }
//    public string? NameAr { get; set; }
//    public string? DescriptionEn { get; set; }
//    public string? DescriptionAr { get; set; }

//    public string? NotesEn { get; set; }
//    public string? NotesAr { get; set; }
//    public string? Code { get; set; }

//    public string? Price { get; set; }

//    public bool IsVegan { get; set; }
//    public bool IsVegetarian { get; set; }
//    public bool IsGlutenFree { get; set; }
//    public bool IsDairyFree { get; set; }
//    public bool IsLowGI { get; set; }
//    public bool IsSweet { get; set; }
//    public bool IsSpicy { get; set; }
//    public bool IsFish { get; set; }
//    public bool IsMeat { get; set; }
//    public bool IsCold { get; set; }
//    public bool IsWarm { get; set; }
//    public string? ImageUrl { get; set; }


//    public virtual List<MealMealType> MealMealTypes { get; set; }
//}

//public class MealMealType : AuditableEntity, IAggregateRoot
//{
//    public DefaultIdType? MealId { get; set; }
//    public Meal? Meal { get; set; }
//    public DefaultIdType? MealTypeId { get; set; }
//    public MealType? MealType { get; set; }
//}
