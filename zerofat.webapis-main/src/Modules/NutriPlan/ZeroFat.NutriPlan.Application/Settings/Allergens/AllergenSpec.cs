using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;
using ZeroFat.NutriPlan.Domain.Settings;





namespace ZeroFat.NutriPlan.Application.Settings.Allergens;
public class AllergensBySearchRequestSpec : EntitiesByPaginationFilterSpec<Allergen, AllergenDto>
{
    public AllergensBySearchRequestSpec(
        SearchAllergensRequest request)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy());
    }
}

public class AllergenByIdSpec<T> : Specification<Allergen, T>
{
    public AllergenByIdSpec(DefaultIdType id) => Query.Where(p => p.Id == id);
}

