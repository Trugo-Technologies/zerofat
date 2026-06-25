using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;

namespace ZeroFat.GymUp.Application.Catalog.BodyParts;
public class UpdateBodyPartRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType? Id { get; set; }
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public bool IsActive { get; set; }
}

public class UpdateBodyPartRequestValidator : CustomValidator<UpdateBodyPartRequest>
{
    public UpdateBodyPartRequestValidator(IReadRepository<BodyPart> repository, IStringLocalizer<UpdateBodyPartRequestValidator> localaizer)
    {

        RuleFor(u => u.NameAr)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (req, name, _) => !await repository.AnyAsync(new ExpressionSpecification<BodyPart>(x => x.NameAr == name && req.Id != x.Id), _))
                 .WithMessage(localaizer["Arabic name already exists"]);

        RuleFor(u => u.NameEn)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (req, name, _) => !await repository.AnyAsync(new ExpressionSpecification<BodyPart>(x => x.NameEn == name && req.Id != x.Id), _))
                .WithMessage(localaizer["English name already exists"]);

    }
}

public class UpdateBodyPartRequestHandler(IRepositoryWithEvents<BodyPart> repository, IStringLocalizer<UpdateBodyPartRequestHandler> localizer) : IRequestHandler<UpdateBodyPartRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<BodyPart> _repository = repository;
    private readonly IStringLocalizer<UpdateBodyPartRequestHandler> _localizer = localizer;

    public async Task<Result<DefaultIdType>> Handle(UpdateBodyPartRequest request, CancellationToken cancellationToken)
    {
        var part = await _repository.GetByIdAsync(request.Id!.Value, cancellationToken);

        _ = part ?? throw new NotFoundException(_localizer["Body part not found"]);

        part.NameAr = request.NameAr;
        part.NameEn = request.NameEn;
        part.IsActive = request.IsActive;

        await _repository.UpdateAsync(part, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(part.Id);
    }
}

