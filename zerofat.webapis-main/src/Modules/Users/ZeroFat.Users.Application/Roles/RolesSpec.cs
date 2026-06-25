using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Specification;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.Users.Application.Roles;
public class ApplicationRolesBySearchRequestSpec : EntitiesByPaginationFilterSpec<ApplicationRole, RoleDto>
{
    public ApplicationRolesBySearchRequestSpec(SearchRolesRequest request)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
             .Where(x => x.UserType == request.UserType, request.UserType.HasValue);
    }
}

public class RoleByNameSpec : Specification<ApplicationRole>
{
    public RoleByNameSpec(string name) => Query.Where(p => p.Name == name);
}
