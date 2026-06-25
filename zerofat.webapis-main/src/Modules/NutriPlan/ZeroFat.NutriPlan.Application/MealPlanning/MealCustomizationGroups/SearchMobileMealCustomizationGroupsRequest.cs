using Mapster;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.NutriPlan.Domain.MealPlanning;

namespace ZeroFat.NutriPlan.Application.MealPlanning.MealCustomizationGroups;
public class SearchMobileMealCustomizationGroupsRequest : IQuery<Result<List<MealCustomizationGroupMobileDto>>>
{
    public DefaultIdType? MealId { get; set; }
}


public class SearchMobbileMealCustomizationGroupsRequestHandler(
    IReadRepository<MealCustomizationGroup> repository) : IQueryHandler<SearchMobileMealCustomizationGroupsRequest, Result<List<MealCustomizationGroupMobileDto>>>
{
    private readonly IReadRepository<MealCustomizationGroup> _repository = repository;

    public async Task<Result<List<MealCustomizationGroupMobileDto>>> Handle(SearchMobileMealCustomizationGroupsRequest request, CancellationToken cancellationToken)
    {
        TypeAdapterConfig config = new();
        config.NewConfig<MealCustomizationGroup, MealCustomizationGroupMobileDto>()
                .Map(destination => destination.Options, src => src.Options.Where(x => x.MealId == request.MealId || x.MealId == null));

        var list = await _repository.ListAsync(new ExpressionSpecificationProjecting<MealCustomizationGroup, MealCustomizationGroupMobileDto>(x => true), config, cancellationToken);

        return await Result<List<MealCustomizationGroupMobileDto>>.SuccessAsync(data : list);
    }
}
