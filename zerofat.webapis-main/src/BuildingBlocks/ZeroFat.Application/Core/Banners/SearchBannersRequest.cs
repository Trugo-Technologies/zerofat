using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Core;

namespace ZeroFat.Application.Core.Banners;
public class SearchBannersRequest : PaginationFilter, IQuery<PaginationResponse<BannerDto>>
{
    public bool? IsActive { get; set; }
}


public class SearchBannersRequestHandler(IReadRepository<Banner> repository) : IRequestHandler<SearchBannersRequest, PaginationResponse<BannerDto>>
{
    private readonly IReadRepository<Banner> _repository = repository;

    public async Task<PaginationResponse<BannerDto>> Handle(SearchBannersRequest request, CancellationToken cancellationToken)
         => await _repository.PaginatedListAsync(new BannersBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);

}
