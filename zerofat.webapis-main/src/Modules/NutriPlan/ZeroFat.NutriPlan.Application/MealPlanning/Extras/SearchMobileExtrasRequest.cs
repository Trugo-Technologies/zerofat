using Mapster;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.Extras;
public class SearchMobileExtrasRequest : IQuery<Result<List<ExtraIngredientMobileDto>>>
{
    public DefaultIdType? MealId { get; set; }
}


public class SearchMobbileExtrasRequestHandler(
    IReadRepository<Extra> repository,
    IReadRepository<Ingredient> ingredientRepo) : IQueryHandler<SearchMobileExtrasRequest, Result<List<ExtraIngredientMobileDto>>>
{
    private readonly IReadRepository<Extra> _repository = repository;
    private readonly IReadRepository<Ingredient> _ingredientRepo = ingredientRepo;

    public async Task<Result<List<ExtraIngredientMobileDto>>> Handle(SearchMobileExtrasRequest request, CancellationToken cancellationToken)
    {
        var extras = await _repository.ListAsync(new ExpressionSpecification<Extra>(x => x.MealId == request.MealId), cancellationToken);

        var list = new List<ExtraIngredientMobileDto>();
        foreach(var extraGroup in extras.GroupBy(x => x.OrginalIngredientId))
        {
            var ingredient = await _ingredientRepo.FirstOrDefaultAsync(new ExpressionSpecification<Ingredient>(x => x.Id == extraGroup.Key), cancellationToken);
            list.Add(new ExtraIngredientMobileDto()
            {
                Id = ingredient.Id,
                NameAr = ingredient.NameAr,
                NameEn = ingredient.NameEn,
                ImageUrl = extraGroup.FirstOrDefault().ImageUrl,
                Extras = extraGroup.ToList().Adapt<List<ExtraMobileDto>>()
            });
        }

        return await Result<List<ExtraIngredientMobileDto>>.SuccessAsync(list);
    }
}
