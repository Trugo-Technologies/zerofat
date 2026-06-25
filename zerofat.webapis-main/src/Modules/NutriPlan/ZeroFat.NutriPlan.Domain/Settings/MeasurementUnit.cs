using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Domain.Settings;
public class MeasurementUnit : AuditableEntity, IAggregateRoot
{
    public string? NameEn { get; set; }
    public string? NameAr { get; set; }
    public string? Code { get; set; }
    public string? IconUrl { get; set; }
    public bool IsDefault { get; set; }

    public virtual List<IngredientMeasurementUnit> IngredientMeasurementUnits { get; set; }
}
