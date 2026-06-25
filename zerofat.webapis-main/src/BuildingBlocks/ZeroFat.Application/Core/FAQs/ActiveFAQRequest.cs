using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Core;

namespace ZeroFat.Application.Core.FAQs;
public class ActiveFaqRequest : ICommand<Result>
{
    public DefaultIdType Id { get; set; }
    public ActiveFaqRequest(DefaultIdType id) => Id = id;
}

public class ActiveFaqRequestHandler(IRepository<Faq> repository, IStringLocalizer<ActiveFaqRequestHandler> localizer) : ICommandHandler<ActiveFaqRequest, Result>
{
    private readonly IRepository<Faq> _repository = repository;
    private readonly IStringLocalizer<ActiveFaqRequestHandler> _localizer = localizer;

    public async Task<Result> Handle(ActiveFaqRequest request, CancellationToken cancellationToken)
    {
        var faq = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = faq ?? throw new NotFoundException(_localizer["FAQ not found"]);


        faq.IsActive = !faq.IsActive;

        await _repository.UpdateAsync(faq, cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
