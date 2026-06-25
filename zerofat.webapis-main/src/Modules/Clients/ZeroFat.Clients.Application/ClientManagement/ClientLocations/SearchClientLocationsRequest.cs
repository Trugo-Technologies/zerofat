using MediatR;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.ClientManagement.ClientLocations;
public class SearchClientLocationsRequest : PaginationFilter, IQuery<PaginationResponse<ClientLocationSimplifyDto>>
{
    public DefaultIdType? ClientId { get; set; }
    public AddressType? Type { get; set; }

}


public class SearchClientLocationsRequestHandler(IReadRepository<ClientLocation> repository, ICurrentUser currentUser) : IRequestHandler<SearchClientLocationsRequest, PaginationResponse<ClientLocationSimplifyDto>>
{
    private readonly IReadRepository<ClientLocation> _repository = repository;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<PaginationResponse<ClientLocationSimplifyDto>> Handle(SearchClientLocationsRequest request, CancellationToken cancellationToken)
    {
        if (_currentUser.GetRoleType()!.Equals(nameof(UserType.Client).ToString(), StringComparison.OrdinalIgnoreCase))
        {
            request.ClientId = _currentUser.GetUserId();
        }

        return await _repository.PaginatedListAsync(new ClientLocationsBySearchRequestSpec(request), request.PageNumber, request.PageSize, cancellationToken);
    }
}
