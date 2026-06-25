using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Core;

namespace ZeroFat.Application.Core.Advertisements;
public class SearchAdvertisementsRequest : PaginationFilter, IQuery<PaginationResponse<AdvertisementDto>>
{
    public bool? IsActive { get; set; }
}


public class SearchAdvertisementsRequestHandler(IReadRepository<Advertisement> repository) : IRequestHandler<SearchAdvertisementsRequest, PaginationResponse<AdvertisementDto>>
{
    private readonly IReadRepository<Advertisement> _repository = repository;

    public async Task<PaginationResponse<AdvertisementDto>> Handle(SearchAdvertisementsRequest request, CancellationToken cancellationToken)
         => await _repository.PaginatedListAsync(new AdvertisementsBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);

}
