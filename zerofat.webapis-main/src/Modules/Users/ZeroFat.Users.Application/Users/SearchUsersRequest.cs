using Mapster;
using MediatR;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Enums;

namespace ZeroFat.Users.Application.Users;
public class SearchUsersRequest : PaginationFilter, IQuery<PaginationResponse<UserDto>>
{
    public UserType? UserType { get; set; }
    public bool? IsActive { get; set; }
}


public class SearchUsersRequestHandler : IRequestHandler<SearchUsersRequest, PaginationResponse<UserDto>>
{
    private readonly IReadRepository<ApplicationUser> _repository;
    public SearchUsersRequestHandler(IReadRepository<ApplicationUser> repository)
    {
        _repository = repository;
    }

    public async Task<PaginationResponse<UserDto>> Handle(SearchUsersRequest request, CancellationToken cancellationToken)
    {
        TypeAdapterConfig config = new TypeAdapterConfig();
        config.NewConfig<ApplicationUser, UserDto>()
                .Map(destination => destination.Roles, src => src.ApplicationUserRoles.Select(x => x.Role.Name).ToList());

        var spec = new ApplicationUsersBySearchRequestSpec(request);
        var users = await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, config, cancellationToken);

        return users;
    }
}
