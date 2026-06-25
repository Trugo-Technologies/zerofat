using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Models;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Core;

namespace ZeroFat.Application.Core.FAQs;
public class CreateFaqRequest : ICommand<Result<DefaultIdType>>
{
    public string Question { get; set; } = default!;
    public string Answer { get; set; } = default!;
    public DefaultIdType FaqCategoryId { get; set; }
    public List<string>? Tags { get; set; }
    public bool IsActive { get; set; }
}

public class CreateFaqRequestValidator : CustomValidator<CreateFaqRequest>
{
    public CreateFaqRequestValidator(IReadRepository<FaqCategory> repository, IStringLocalizer<CreateFaqRequestValidator> localaizer)
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


public class CreateFaqRequestHandler(IRepository<Faq> repo) : IRequestHandler<CreateFaqRequest, Result<DefaultIdType>>
{
    private readonly IRepository<Faq> _repo = repo;

    public async Task<Result<DefaultIdType>> Handle(CreateFaqRequest request, CancellationToken cancellationToken)
    {
        var faq = new Faq
        {
            Answer = request.Answer,
            Question = request.Question,
            Tags = request.Tags,
            FaqCategoryId = request.FaqCategoryId,
            IsActive = request.IsActive
        };

        await _repo.AddAsync(faq, cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(faq.Id);
    }

}
