using ZeroFat.Application.Common.CQRS;
using ZeroFat.ClientPortal.Application.Contracts;
using MediatR;

namespace ZeroFat.ClientPortal.Infrastructure.Services;
internal sealed class ClientPortalModule : IClientPortalModule
{
    private readonly IMediator _mediator;

    public ClientPortalModule(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task ExecuteCommandAsync(ICommand command)
    {
        await _mediator.Send(command);
    }

    public async Task<TResult> ExecuteCommandAsync<TResult>(ICommand<TResult> command)
    {
        return await _mediator.Send(command);
    }

    public async Task<TResult> ExecuteQueryAsync<TResult>(IQuery<TResult> query)
    {
        return await _mediator.Send(query);
    }
}
