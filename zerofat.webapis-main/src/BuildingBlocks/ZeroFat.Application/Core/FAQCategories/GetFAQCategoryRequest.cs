using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Domain.Core;

namespace ZeroFat.Application.Core.FAQCategories;
public class GetFaqCategoryRequest(DefaultIdType id) : IQuery<Result<FaqCategoryDetailsDto>>
{
    public DefaultIdType Id { get; set; } = id;
}

public class GetFaqCategoryRequestHandler(IRepositoryWithEvents<FaqCategory> repository, IStringLocalizer<GetFaqCategoryRequestHandler> localizer) : IRequestHandler<GetFaqCategoryRequest, Result<FaqCategoryDetailsDto>>
{
    private readonly IRepositoryWithEvents<FaqCategory> _repository = repository;
    private readonly IStringLocalizer _localizer = localizer;

    public async Task<Result<FaqCategoryDetailsDto>> Handle(GetFaqCategoryRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.FirstOrDefaultAsync(new FaqCategoryByIdSpec<FaqCategoryDetailsDto>(request.Id), cancellationToken);
        _ = entity ?? throw new NotFoundException(_localizer["FAQCategory not found", request.Id]);

        return await Result<FaqCategoryDetailsDto>.SuccessAsync(entity);
    }
}
