using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Enums;

namespace ZeroFat.Users.Application.Roles;
public class SearchRolesRequest : PaginationFilter, IQuery<PaginationResponse<RoleDto>>
{
    public UserType? UserType { get; set; }
}

public class SearchRolesRequestHandler : IQueryHandler<SearchRolesRequest, PaginationResponse<RoleDto>>
{
    private readonly IReadRepository<ApplicationRole> _repository;
    public SearchRolesRequestHandler(
        IReadRepository<ApplicationRole> repository)
    {
        _repository = repository;
    }

    public async Task<PaginationResponse<RoleDto>> Handle(SearchRolesRequest request, CancellationToken cancellationToken)
    {
        var spec = new ApplicationRolesBySearchRequestSpec(request);
        var users = await _repository.PaginatedListAsync(spec, request.PageNumber, request.PageSize, cancellationToken);

        return users;
    }
}
