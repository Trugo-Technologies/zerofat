using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;
using ZeroFat.NutriPlan.Domain;
using ZeroFat.NutriPlan.Domain.Settings;

namespace ZeroFat.NutriPlan.Application.Settings.Allergens;
public class UpdateAllergenRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType? Id { get; set; }
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public IFormFile? Icon { get; set; }
}

public class UpdateAllergenRequestValidator : CustomValidator<UpdateAllergenRequest>
{
    public UpdateAllergenRequestValidator(IReadRepository<Allergen> repository, IStringLocalizer<UpdateAllergenRequestValidator> localaizer)
    {

        RuleFor(u => u.NameAr)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (req, name, _) => !await repository.AnyAsync(new ExpressionSpecification<Allergen>(x => x.NameAr == name && req.Id != x.Id), _))
                 .WithMessage(localaizer["Arabic name already exists"]);

        RuleFor(u => u.NameEn)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (req, name, _) => !await repository.AnyAsync(new ExpressionSpecification<Allergen>(x => x.NameEn == name && req.Id != x.Id), _))
                .WithMessage(localaizer["English name already exists"]);

    }
}

public class UpdateAllergenRequestHandler(IRepositoryWithEvents<Allergen> repository, IFileStorageManager fileStorageManager, IStringLocalizer<UpdateAllergenRequestHandler> localizer) : IRequestHandler<UpdateAllergenRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<Allergen> _repository = repository;
    private readonly IStringLocalizer<UpdateAllergenRequestHandler> _localizer = localizer;
    private readonly IFileStorageManager _fileStorageManager = fileStorageManager;

    public async Task<Result<DefaultIdType>> Handle(UpdateAllergenRequest request, CancellationToken cancellationToken)
    {
        var part = await _repository.GetByIdAsync(request.Id!.Value, cancellationToken);

        _ = part ?? throw new NotFoundException(_localizer["Allergen not found"]);

        part.NameAr = request.NameAr;
        part.NameEn = request.NameEn;
        part.IconUrl = request.Icon == null ? part.IconUrl : await _fileStorageManager.UploadAsync<Category>(request.Icon, FileType.Image, ModuleConstant.ModuleName, cancellationToken);

        await _repository.UpdateAsync(part, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(part.Id);
    }
}

