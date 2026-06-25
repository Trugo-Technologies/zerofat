using ZeroFat.Application.Common.CQRS;
using MediatR;
using ZeroFat.Application.Core.Contracts;

namespace ZeroFat.Infrastructure.Core.Services;
internal sealed class CoreModule : ICoreModule
{
    private readonly IMediator _mediator;

    public CoreModule(IMediator mediator)
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
