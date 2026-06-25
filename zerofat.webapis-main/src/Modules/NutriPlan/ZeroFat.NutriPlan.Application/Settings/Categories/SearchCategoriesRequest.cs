using MediatR;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.Settings.Categories;
public class SearchCategoriesRequest : PaginationFilter, IQuery<PaginationResponse<CategoryDto>>
{
    public CategoryType? CategoryType { get; set; }
}


public class SearchCategoriesRequestHandler(IReadRepository<Category> repository) : IQueryHandler<SearchCategoriesRequest, PaginationResponse<CategoryDto>>
{
    private readonly IReadRepository<Category> _repository = repository;

    public async Task<PaginationResponse<CategoryDto>> Handle(SearchCategoriesRequest request, CancellationToken cancellationToken)
         => await _repository.PaginatedListAsync(new CategoriesBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);

}
