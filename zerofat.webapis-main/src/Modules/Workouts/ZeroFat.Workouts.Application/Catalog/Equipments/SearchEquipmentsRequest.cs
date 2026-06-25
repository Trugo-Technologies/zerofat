using MediatR;
using ZeroFat.Application.Common.Persistence;

namespace ZeroFat.GymUp.Application.Catalog.Equipments;
public class SearchEquipmentsRequest : PaginationFilter, IQuery<PaginationResponse<EquipmentDto>>
{
    public bool? IsActive { get; set; }
    public DefaultIdType? CategoryId { get; set; }
}


public class SearchEquipmentsRequestHandler(IReadRepository<Equipment> repository) : IRequestHandler<SearchEquipmentsRequest, PaginationResponse<EquipmentDto>>
{
    private readonly IReadRepository<Equipment> _repository = repository;

    public async Task<PaginationResponse<EquipmentDto>> Handle(SearchEquipmentsRequest request, CancellationToken cancellationToken)
         => await _repository.PaginatedListAsync(new EquipmentsBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);

}
