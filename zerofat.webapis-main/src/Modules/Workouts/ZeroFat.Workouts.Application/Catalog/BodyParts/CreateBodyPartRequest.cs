using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;

namespace ZeroFat.GymUp.Application.Catalog.BodyParts;
public class CreateBodyPartRequest : ICommand<Result<DefaultIdType>>
{
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public bool? IsActive { get; set; }
}

public class CreateBodyPartRequestValidator : CustomValidator<CreateBodyPartRequest>
{
    public CreateBodyPartRequestValidator(IReadRepository<BodyPart> repository, IStringLocalizer<CreateBodyPartRequestValidator> localaizer)
    {

        RuleFor(u => u.NameAr)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (name, _) => !await repository.AnyAsync(new ExpressionSpecification<BodyPart>(x => x.NameAr == name), _))
                 .WithMessage(localaizer["Arabic name already exists"]);

        RuleFor(u => u.NameEn)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (name, _) => !await repository.AnyAsync(new ExpressionSpecification<BodyPart>(x => x.NameEn == name), _))
                .WithMessage(localaizer["English name already exists"]);

    }
}


public class CreateBodyPartRequestHandler(IRepositoryWithEvents<BodyPart> repository) : IRequestHandler<CreateBodyPartRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<BodyPart> _repository = repository;

    public async Task<Result<DefaultIdType>> Handle(CreateBodyPartRequest request, CancellationToken cancellationToken)
    {
        var part = new BodyPart
        {
            NameEn = request.NameEn,
            NameAr = request.NameAr,
            IsActive = request.IsActive ?? false
        };

        await _repository.AddAsync(part, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(part.Id);
    }

}
