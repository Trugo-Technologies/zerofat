using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.ClientPortal.Domain.ClientManagement;

namespace ZeroFat.ClientPortal.Application.ClientManagement;

public class DeleteClientLocationRequest : ICommand<Result<Guid>>
{
    public Guid Id { get; set; }
    public DeleteClientLocationRequest(Guid id) => Id = id;
}


public class DeleteClientLocationRequestHandler(IRepository<ClientLocation> repository, IStringLocalizer<DeleteClientLocationRequestHandler> localizer) : IRequestHandler<DeleteClientLocationRequest, Result<Guid>>
{
    private readonly IRepository<ClientLocation> _repository = repository;
    private readonly IStringLocalizer<DeleteClientLocationRequestHandler> _localizer = localizer;

    public async Task<Result<Guid>> Handle(DeleteClientLocationRequest request, CancellationToken cancellationToken)
    {
        var location = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = location ?? throw new NotFoundException(_localizer["Location not found"]);

        await _repository.DeleteAsync(location, cancellationToken);

        return await Result<Guid>.SuccessAsync(location.Id);
    }

}
