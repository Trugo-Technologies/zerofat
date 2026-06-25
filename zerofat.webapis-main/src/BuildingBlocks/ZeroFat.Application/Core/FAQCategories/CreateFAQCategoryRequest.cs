using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Core;

namespace ZeroFat.Application.Core.FAQCategories;
public class CreateFaqCategoryRequest : ICommand<Result<DefaultIdType>>
{
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public bool? IsActive { get; set; }
}

public class CreateFaqCategoryRequestValidator : CustomValidator<CreateFaqCategoryRequest>
{
    public CreateFaqCategoryRequestValidator(IReadRepository<FaqCategory> repository, IStringLocalizer<CreateFaqCategoryRequestValidator> localaizer)
    {

        RuleFor(u => u.NameAr)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (name, _) => !await repository.AnyAsync(new ExpressionSpecification<FaqCategory>(x => x.NameAr == name), _))
                 .WithMessage(localaizer["Arabic name already exists"]);

        RuleFor(u => u.NameEn)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (name, _) => !await repository.AnyAsync(new ExpressionSpecification<FaqCategory>(x => x.NameEn == name), _))
                .WithMessage(localaizer["English name already exists"]);

    }
}


public class CreateFaqCategoryRequestHandler(IRepository<FaqCategory> repo) : IRequestHandler<CreateFaqCategoryRequest, Result<DefaultIdType>>
{
    private IRepository<FaqCategory> _repo = repo;

    public async Task<Result<DefaultIdType>> Handle(CreateFaqCategoryRequest request, CancellationToken cancellationToken)
    {
        var cate = new FaqCategory
        {
            NameEn = request.NameEn,
            NameAr = request.NameAr,
            DescriptionAr = request.DescriptionAr,
            DescriptionEn = request.DescriptionEn,
            IsActive = request.IsActive ?? false
        };

        await _repo.AddAsync(cate, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(cate.Id);
    }

}
