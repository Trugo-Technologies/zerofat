using Ardalis.Specification;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.ClientPortal.Application.SubscriptionManagement.DailySelections;
using ZeroFat.ClientPortal.Infrastructure.Persistence.Context;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.ClientPortal.Infrastructure.Kitchen;
public class SearchMealByCountRequest : PaginationFilter, IQuery<Result<List<MealRequestReport>>>
{
    public DateOnly? Date { get; set; }
    public DefaultIdType? ClientId { get; set; }
}


public class SearchMealByCountRequestHandler(ClientPortalContext clientPortalContext, IRepository<CustomMeal> customMealRepo) : IQueryHandler<SearchMealByCountRequest, Result<List<MealRequestReport>>>
{

    public async Task<Result<List<MealRequestReport>>> Handle(SearchMealByCountRequest request, CancellationToken cancellationToken)
    {

        var query = clientPortalContext.DailyMealSelections
            .Where(d => d.MealId != null && d.DailySelection.DailySelectionStatus != DailySelectionStatus.Paused);

        if(request.Date.HasValue)
        {
            query = query.Where(d => d.Date == request.Date.Value);
        }

        if (request.ClientId.HasValue)
        {
            query = query.Where(d => d.ClientId == request.ClientId);
        }

        TypeAdapterConfig config = new TypeAdapterConfig();
        config.NewConfig<Meal, MealReprotDto>()
        .Map(destination => destination.Allergens, src => src.Allergens.Select(x => x.Allergen));

        // Make sure Meal is not null
        var result = await query.GroupBy(d => new { d.Date, d.MealId, d.CustomMealId }) // Group by Date and Meal
            .Select(g => new MealRequestReport
            {
                Id = Guid.NewGuid(),
                Meal = g.FirstOrDefault().Meal.Adapt<MealReprotDto>(config),
                RequestCount = g.Count(),
                Date = g.Key.Date,
                CustomMealId = g.Key.CustomMealId,
            })
            .OrderBy(r => r.Date) // Order by Date
            .ToListAsync(cancellationToken: cancellationToken);

        foreach(var item in result)
        {
            if (item.CustomMealId.HasValue)
            {
                item.Meal = await customMealRepo.FirstOrDefaultAsync(new ExpressionSpecificationProjecting<CustomMeal, MealReprotDto>(x => x.Id == item.CustomMealId), cancellationToken);
            }
        }

        return await Result<List<MealRequestReport>>.SuccessAsync(result);
    }
}

public class MealRequestReport : IDto
{
    public MealReprotDto? Meal { get; set; } // Date of the meal selection
    public int? RequestCount { get; set; } // Special notes or preferences for the meal
    public DateOnly? Date { get; set; }
     public Guid? CustomMealId { get; set; }
    public Guid? Id { get; set; }
}


public class MealReprotDto : IDto
{
    public string? NameEn { get; set; } // Name of the meal (e.g., "Grilled Chicken Salad")
    public string? NameAr { get; set; } // Name of the meal (e.g., "Grilled Chicken Salad")
    public string? ImageUrl { get; set; }


    // Navigation properties

    public List<AllergenDto> Allergens { get; set; }
}
