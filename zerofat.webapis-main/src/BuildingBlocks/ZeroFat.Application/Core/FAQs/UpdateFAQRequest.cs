using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Core;

namespace ZeroFat.Application.Core.FAQs;
public class UpdateFaqRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType? Id { get; set; }
    public string Question { get; set; } = default!;
    public string Answer { get; set; } = default!;
    public DefaultIdType FaqCategoryId { get; set; }
    public List<string>? Tags { get; set; }
    public bool IsActive { get; set; }
}

public class UpdateFaqRequestValidator : CustomValidator<UpdateFaqRequest>
{
    public UpdateFaqRequestValidator(IReadRepository<FaqCategory> repository, IStringLocalizer<UpdateFaqRequestValidator> localaizer)
    {

        RuleFor(u => u.FaqCategoryId)
          .Cascade(CascadeMode.Stop)
          .NotEmpty()
          .MustAsync(async (id, _) => await repository.AnyAsync(new ExpressionSpecification<FaqCategory>(x => x.Id == id), _))
               .WithMessage(localaizer["Category not found"]);

        RuleFor(u => u.Question)
           .Cascade(CascadeMode.Stop)
           .NotEmpty();

        RuleFor(u => u.Answer)
           .Cascade(CascadeMode.Stop)
           .NotEmpty();

    }
}

public class UpdateFaqRequestHandler(IRepository<Faq> repository, IStringLocalizer<UpdateFaqRequestHandler> localizer) : IRequestHandler<UpdateFaqRequest, Result<DefaultIdType>>
{
    private readonly IRepository<Faq> _repository = repository;
    private readonly IStringLocalizer<UpdateFaqRequestHandler> _localizer = localizer;


    public async Task<Result<DefaultIdType>> Handle(UpdateFaqRequest request, CancellationToken cancellationToken)
    {
        var faq = await _repository.GetByIdAsync(request.Id!.Value, cancellationToken);

        _ = faq ?? throw new NotFoundException(_localizer["FAQ not found"]);

        faq.Question = request.Question;
        faq.Answer = request.Answer;
        faq.Tags = request.Tags;
        faq.FaqCategoryId = request.FaqCategoryId;
        faq.IsActive = request.IsActive;


        await _repository.UpdateAsync(faq, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(faq.Id);
    }
}

