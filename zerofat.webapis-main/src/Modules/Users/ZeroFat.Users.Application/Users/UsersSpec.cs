using Ardalis.Specification;
using ZeroFat.Application.Common.Specification;

namespace ZeroFat.Users.Application.Users;
public class ApplicationUsersBySearchRequestSpec : EntitiesByPaginationFilterSpec<ApplicationUser, UserDto>
{
    public ApplicationUsersBySearchRequestSpec(SearchUsersRequest request)
        : base(request)
    {
        Query.OrderByDescending(c => c.CreatedOn, !request.HasOrderBy())
             .Where(x => x.IsActive == request.IsActive, request.IsActive.HasValue)
             .Where(x => !x.IsTest)
             .Where(x => x.UserType == request.UserType, request.UserType.HasValue);
    }
}

public class UserByEmailSpec : Specification<ApplicationUser>
{
    public UserByEmailSpec(string email, string? exceptId = null) => Query.Where(p => p.NormalizedEmail == email.Normalize() && p.Id != exceptId);
}

public class UserByUserNameSpec : Specification<ApplicationUser>
{
    public UserByUserNameSpec(string userName, string? exceptId = null) => Query.Where(p => p.NormalizedUserName == userName.Normalize() && p.Id != exceptId);
}

public class UserByPhoneNumberSpec : Specification<ApplicationUser>
{
    public UserByPhoneNumberSpec(string phoneNumber, string? exceptId = null) => Query.Where(p => p.PhoneNumber == phoneNumber && p.Id != exceptId);
}

public class UserByIdRequestSpec<T> : Specification<ApplicationUser, T>
{
    public UserByIdRequestSpec(string id) => Query.Where(p => p.Id == id);
}
