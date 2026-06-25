using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.NutrientsAttributes;
public class CreateNutrientsAttributeRequest : ICommand<Result<Guid>>
{
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public string Unit { get; set; } = default!;
}

public class CreateNutrientsAttributeRequestValidator : CustomValidator<CreateNutrientsAttributeRequest>
{
    public CreateNutrientsAttributeRequestValidator(IReadRepository<NutrientsAttribute> repository, IStringLocalizer<CreateNutrientsAttributeRequestValidator> localaizer)
    {

        RuleFor(u => u.NameAr)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (name, _) => !await repository.AnyAsync(new ExpressionSpecification<NutrientsAttribute>(x => x.NameAr == name), _))
                 .WithMessage(localaizer["Arabic name already exists"]);


        RuleFor(u => u.NameEn)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (name, _) => !await repository.AnyAsync(new ExpressionSpecification<NutrientsAttribute>(x => x.NameEn == name), _))
                .WithMessage(localaizer["English name already exists"]);

    }
}


public class CreateNutrientsAttributeRequestHandler(IRepositoryWithEvents<NutrientsAttribute> repository, IFileStorageManager fileStorageManager) : ICommandHandler<CreateNutrientsAttributeRequest, Result<Guid>>
{
    private readonly IRepositoryWithEvents<NutrientsAttribute> _repository = repository;
    private readonly IFileStorageManager _fileStorageManager = fileStorageManager;

    public async Task<Result<Guid>> Handle(CreateNutrientsAttributeRequest request, CancellationToken cancellationToken)
    {
        var part = new NutrientsAttribute
        {
            NameEn = request.NameEn,
            NameAr = request.NameAr,
            Unit = request.Unit,
            // IconUrl = request.Icon == null ? null : await _fileStorageManager.UploadAsync<NutrientsAttribute>(request.Icon, FileType.Image, ModuleConstant.ModuleName, cancellationToken),
        };

        await _repository.AddAsync(part, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<Guid>.SuccessAsync(part.Id);
    }

}
