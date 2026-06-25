using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ZeroFat.Users.Application.Users;
public class Enable2FAUserRequest : ICommand<Result>
{
    public Guid UserId { get; set; }
    public bool TwoFactorEnabled { get; set; }
}

public class Enable2FAUserRequestHandler : ICommandHandler<Enable2FAUserRequest, Result>
{
    private readonly UserManager<ApplicationUser> _repository;

    public Enable2FAUserRequestHandler(UserManager<ApplicationUser> repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(Enable2FAUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _repository.Users.FirstOrDefaultAsync(x => x.PublicId == request.UserId, cancellationToken);
        _ = user ?? throw new NotFoundException("User Not Found");

        user.TwoFactorEnabled = request.TwoFactorEnabled;
        await _repository.UpdateAsync(user);

        return (Result)await Result.SuccessAsync();
    }
}
