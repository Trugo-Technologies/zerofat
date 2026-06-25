using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Core;
using ZeroFat.Domain.Enums;

namespace ZeroFat.Application.Core.FAQCategories;
public class UpdateFaqCategoryRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType? Id { get; set; }
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public bool IsActive { get; set; }
}

public class UpdateFaqCategoryRequestValidator : CustomValidator<UpdateFaqCategoryRequest>
{
    public UpdateFaqCategoryRequestValidator(IReadRepository<FaqCategory> repository, IStringLocalizer<UpdateFaqCategoryRequestValidator> localaizer)
    {

        RuleFor(u => u.NameAr)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (req, name, _) => !await repository.AnyAsync(new ExpressionSpecification<FaqCategory>(x => x.NameAr == name && req.Id != x.Id), _))
                 .WithMessage(localaizer["Arabic name already exists"]);

        RuleFor(u => u.NameEn)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (req, name, _) => !await repository.AnyAsync(new ExpressionSpecification<FaqCategory>(x => x.NameEn == name && req.Id != x.Id), _))
                .WithMessage(localaizer["English name already exists"]);

    }
}

public class UpdateFaqCategoryRequestHandler(IRepository<FaqCategory> repository, IStringLocalizer<UpdateFaqCategoryRequestHandler> localizer) : IRequestHandler<UpdateFaqCategoryRequest, Result<DefaultIdType>>
{
    private readonly IRepository<FaqCategory> _repository = repository;
    private readonly IStringLocalizer<UpdateFaqCategoryRequestHandler> _localizer = localizer;


    public async Task<Result<DefaultIdType>> Handle(UpdateFaqCategoryRequest request, CancellationToken cancellationToken)
    {
        var cate = await _repository.GetByIdAsync(request.Id!.Value, cancellationToken);

        _ = cate ?? throw new NotFoundException(_localizer["Category not found"]);

        cate.NameAr = request.NameAr;
        cate.NameEn = request.NameEn;
        cate.DescriptionAr = request.DescriptionAr;
        cate.DescriptionEn = request.DescriptionEn;
        cate.IsActive = request.IsActive;


        await _repository.UpdateAsync(cate, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(cate.Id);
    }
}

