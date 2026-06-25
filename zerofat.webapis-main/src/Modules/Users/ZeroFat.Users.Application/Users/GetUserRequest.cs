
using Mapster;
using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;

namespace ZeroFat.Users.Application.Users;

public class GetUserRequest(string id) : ICommand<Result<UserDto>>
{
    public string Id { get; set; } = id;
}
public class GetUserRequestHandler(IRepository<ApplicationUser> repository, IStringLocalizer<GetUserRequestHandler> localizer) : IRequestHandler<GetUserRequest, Result<UserDto>>
{
    private readonly IRepository<ApplicationUser> _repository = repository;
    private readonly IStringLocalizer<GetUserRequestHandler> _localizer = localizer;

    public async Task<Result<UserDto>> Handle(GetUserRequest request, CancellationToken cancellationToken)
    {
        TypeAdapterConfig config = new TypeAdapterConfig();
        config.NewConfig<ApplicationUser, UserDto>()
                .Map(destination => destination.Roles, src => src.ApplicationUserRoles.Select(x => x.Role.Name).ToList());

        var user = await _repository.FirstOrDefaultAsync(new UserByIdRequestSpec<UserDto>(request.Id), config, cancellationToken);
        _ = user ?? throw new NotFoundException(_localizer["User not found"]);
        return await Result<UserDto>.SuccessAsync(user);
    }
}
