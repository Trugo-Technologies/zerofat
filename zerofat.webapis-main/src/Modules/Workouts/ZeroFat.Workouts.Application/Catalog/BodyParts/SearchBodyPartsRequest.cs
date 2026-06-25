using MediatR;
using ZeroFat.Application.Common.Persistence;

namespace ZeroFat.GymUp.Application.Catalog.BodyParts;
public class SearchBodyPartsRequest : PaginationFilter, IQuery<PaginationResponse<BodyPartDto>>
{
    public bool? IsActive { get; set; }
}


public class SearchBodyPartsRequestHandler(IReadRepository<BodyPart> repository) : IRequestHandler<SearchBodyPartsRequest, PaginationResponse<BodyPartDto>>
{
    private readonly IReadRepository<BodyPart> _repository = repository;

    public async Task<PaginationResponse<BodyPartDto>> Handle(SearchBodyPartsRequest request, CancellationToken cancellationToken)
         => await _repository.PaginatedListAsync(new BodyPartsBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);

}
