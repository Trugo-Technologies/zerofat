using MediatR;
using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Core;

namespace ZeroFat.Application.Core.PhysicalActivityLevels;
public class SearchPhysicalActivityLevelsRequest : PaginationFilter, IQuery<PaginationResponse<PhysicalActivityLevelDto>>
{
    public bool? IsActive { get; set; }
}


public class SearchPhysicalActivityLevelsRequestHandler(IReadRepository<PhysicalActivityLevel> repository) : IRequestHandler<SearchPhysicalActivityLevelsRequest, PaginationResponse<PhysicalActivityLevelDto>>
{
    private readonly IReadRepository<PhysicalActivityLevel> _repository = repository;

    public async Task<PaginationResponse<PhysicalActivityLevelDto>> Handle(SearchPhysicalActivityLevelsRequest request, CancellationToken cancellationToken)
         => await _repository.PaginatedListAsync(new PhysicalActivityLevelsBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);

}
