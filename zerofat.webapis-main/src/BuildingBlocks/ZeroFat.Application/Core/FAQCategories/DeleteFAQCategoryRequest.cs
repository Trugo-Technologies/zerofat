using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Domain.Core;

namespace ZeroFat.Application.Core.FAQCategories;

public class DeleteFaqCategoryRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType Id { get; set; }
    public DeleteFaqCategoryRequest(DefaultIdType id) => Id = id;
}


public class DeleteFaqCategoryRequestHandler(IRepository<FaqCategory> repository, IStringLocalizer<DeleteFaqCategoryRequestHandler> localizer, IReadRepository<Faq> FaqRepo) : IRequestHandler<DeleteFaqCategoryRequest, Result<DefaultIdType>>
{
    private readonly IRepository<FaqCategory> _repository = repository;
    private readonly IReadRepository<Faq> _FaqRepo = FaqRepo;
    private readonly IStringLocalizer<DeleteFaqCategoryRequestHandler> _localizer = localizer;

    public async Task<Result<DefaultIdType>> Handle(DeleteFaqCategoryRequest request, CancellationToken cancellationToken)
    {
        var cate = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = cate ?? throw new NotFoundException(_localizer["Category not found"]);

        bool used = await _FaqRepo.AnyAsync(new ExpressionSpecification<Faq>(x => x.FaqCategoryId == cate.Id), cancellationToken);
        if (used)
            throw new BadRequestException(_localizer["Can not be deleted, because it's linked with FAQs"]);

        await _repository.DeleteAsync(cate, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(cate.Id);
    }

}
