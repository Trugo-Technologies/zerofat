using MediatR;
using ZeroFat.Application.Common.Persistence;

namespace ZeroFat.GymUp.Application.Creator.WorkoutEquipments;
public class SearchWorkoutEquipmentsRequest : PaginationFilter, IQuery<PaginationResponse<WorkoutEquipmentSimplifyDto>>
{
    public DefaultIdType? WorkoutId { get; set; }
    public DefaultIdType? EquipmentId { get; set; }
    public bool? IsOptional { get; set; }
}


public class SearchWorkoutEquipmentsRequestHandler(IReadRepository<WorkoutEquipment> repository) : IRequestHandler<SearchWorkoutEquipmentsRequest, PaginationResponse<WorkoutEquipmentSimplifyDto>>
{
    private readonly IReadRepository<WorkoutEquipment> _repository = repository;

    public async Task<PaginationResponse<WorkoutEquipmentSimplifyDto>> Handle(SearchWorkoutEquipmentsRequest request, CancellationToken cancellationToken)
         => await _repository.PaginatedListAsync(new WorkoutEquipmentsBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);

}
