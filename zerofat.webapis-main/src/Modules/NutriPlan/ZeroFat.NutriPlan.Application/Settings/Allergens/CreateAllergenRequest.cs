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

namespace ZeroFat.NutriPlan.Application.Settings.Allergens;
public class CreateAllergenRequest : ICommand<Result<DefaultIdType>>
{
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public IFormFile? Icon { get; set; }
}

public class CreateAllergenRequestValidator : CustomValidator<CreateAllergenRequest>
{
    public CreateAllergenRequestValidator(IReadRepository<Allergen> repository, IStringLocalizer<CreateAllergenRequestValidator> localaizer)
    {

        RuleFor(u => u.NameAr)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (name, _) => !await repository.AnyAsync(new ExpressionSpecification<Allergen>(x => x.NameAr == name), _))
                 .WithMessage(localaizer["Arabic name already exists"]);

        RuleFor(u => u.NameEn)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (name, _) => !await repository.AnyAsync(new ExpressionSpecification<Allergen>(x => x.NameEn == name), _))
                .WithMessage(localaizer["English name already exists"]);

    }
}


public class CreateAllergenRequestHandler(IRepositoryWithEvents<Allergen> repository, IFileStorageManager fileStorageManager) : IRequestHandler<CreateAllergenRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<Allergen> _repository = repository;
    private readonly IFileStorageManager _fileStorageManager = fileStorageManager;

    public async Task<Result<DefaultIdType>> Handle(CreateAllergenRequest request, CancellationToken cancellationToken)
    {
        var part = new Allergen
        {
            NameEn = request.NameEn,
            NameAr = request.NameAr,
            IconUrl = request.Icon == null ? null : await _fileStorageManager.UploadAsync<Category>(request.Icon, FileType.Image, ModuleConstant.ModuleName, cancellationToken),
            //IconUrl = 
        };

        await _repository.AddAsync(part, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(part.Id);
    }

}
