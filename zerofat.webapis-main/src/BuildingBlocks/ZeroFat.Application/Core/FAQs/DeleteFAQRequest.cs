using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Domain.Core;

namespace ZeroFat.Application.Core.FAQs;

public class DeleteFaqRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType Id { get; set; }
    public DeleteFaqRequest(DefaultIdType id) => Id = id;
}


public class DeleteFaqRequestHandler(IRepository<Faq> repository, IStringLocalizer<DeleteFaqRequestHandler> localizer) : IRequestHandler<DeleteFaqRequest, Result<DefaultIdType>>
{
    private readonly IRepository<Faq> _repository = repository;
    private readonly IStringLocalizer<DeleteFaqRequestHandler> _localizer = localizer;

    public async Task<Result<DefaultIdType>> Handle(DeleteFaqRequest request, CancellationToken cancellationToken)
    {
        var faq = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = faq ?? throw new NotFoundException(_localizer["FAQ not found"]);

        faq.Tags?.Clear();

        await _repository.DeleteAsync(faq, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(faq.Id);
    }

}
