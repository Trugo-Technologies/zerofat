using MediatR;
using ZeroFat.Application.Common.Persistence;

namespace ZeroFat.GymUp.Application.Catalog.EquipmentCategories;
public class SearchEquipmentCategoriesRequest : PaginationFilter, IQuery<PaginationResponse<EquipmentCategoryDto>>
{
    public bool? IsActive { get; set; }
}


public class SearchEquipmentCategorysRequestHandler(IReadRepository<EquipmentCategory> repository) : IRequestHandler<SearchEquipmentCategoriesRequest, PaginationResponse<EquipmentCategoryDto>>
{
    private readonly IReadRepository<EquipmentCategory> _repository = repository;

    public async Task<PaginationResponse<EquipmentCategoryDto>> Handle(SearchEquipmentCategoriesRequest request, CancellationToken cancellationToken)
         => await _repository.PaginatedListAsync(new EquipmentCategoriesBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);

}
