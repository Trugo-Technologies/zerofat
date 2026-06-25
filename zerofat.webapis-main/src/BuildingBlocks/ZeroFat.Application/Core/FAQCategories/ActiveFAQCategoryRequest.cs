using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Core;

namespace ZeroFat.Application.Core.FAQCategories;
public class ActiveFaqCategoryRequest : ICommand<Result>
{
    public DefaultIdType Id { get; set; }
    public ActiveFaqCategoryRequest(DefaultIdType id) => Id = id;
}

public class ActiveFaqCategoryRequestHandler(IRepository<FaqCategory> repository, IStringLocalizer<ActiveFaqCategoryRequestHandler> localizer) : ICommandHandler<ActiveFaqCategoryRequest, Result>
{
    private readonly IRepository<FaqCategory> _repository = repository;
    private readonly IStringLocalizer<ActiveFaqCategoryRequestHandler> _localizer = localizer;

    public async Task<Result> Handle(ActiveFaqCategoryRequest request, CancellationToken cancellationToken)
    {
        var cate = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = cate ?? throw new NotFoundException(_localizer["Category not found"]);


        cate.IsActive = !cate.IsActive;

        await _repository.UpdateAsync(cate, cancellationToken);

        return (Result)await Result.SuccessAsync();
    }
}
