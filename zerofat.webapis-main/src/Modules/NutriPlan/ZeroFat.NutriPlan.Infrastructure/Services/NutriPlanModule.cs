using ZeroFat.Application.Common.CQRS;
using ZeroFat.NutriPlan.Application.Contracts;
using MediatR;

namespace ZeroFat.NutriPlan.Infrastructure.Services;
internal sealed class NutriPlanModule : INutriPlanModule
{
    private readonly IMediator _mediator;

    public NutriPlanModule(IMediator mediator)
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
