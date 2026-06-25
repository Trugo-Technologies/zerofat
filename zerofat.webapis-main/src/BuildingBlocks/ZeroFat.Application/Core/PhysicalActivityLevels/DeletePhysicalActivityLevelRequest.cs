using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Core;

namespace ZeroFat.Application.Core.PhysicalActivityLevels;

public class DeletePhysicalActivityLevelRequest : ICommand<Result<Guid>>
{
    public Guid Id { get; set; }
    public DeletePhysicalActivityLevelRequest(Guid id) => Id = id;
}


public class DeletePhysicalActivityLevelRequestHandler(IRepository<PhysicalActivityLevel> repository, IStringLocalizer<DeletePhysicalActivityLevelRequestHandler> localizer) : IRequestHandler<DeletePhysicalActivityLevelRequest, Result<Guid>>
{
    private readonly IRepository<PhysicalActivityLevel> _repository = repository;
    private readonly IStringLocalizer<DeletePhysicalActivityLevelRequestHandler> _localizer = localizer;

    public async Task<Result<Guid>> Handle(DeletePhysicalActivityLevelRequest request, CancellationToken cancellationToken)
    {
        var activity = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = activity ?? throw new NotFoundException(_localizer["Physical Activity Level not found"]);

        await _repository.DeleteAsync(activity, cancellationToken);

        return await Result<Guid>.SuccessAsync(activity.Id);
    }

}
