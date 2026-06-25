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

namespace ZeroFat.NutriPlan.Application.NutrientsAttributes;
public class UpdateNutrientsAttributeRequest : ICommand<Result<Guid>>
{
    public Guid? Id { get; set; }
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public string Unit { get; set; } = default!;
    // public IFormFile? Icon { get; set; }
}

public class UpdateNutrientsAttributeRequestValidator : CustomValidator<UpdateNutrientsAttributeRequest>
{
    public UpdateNutrientsAttributeRequestValidator(IReadRepository<NutrientsAttribute> repository, IStringLocalizer<UpdateNutrientsAttributeRequestValidator> localaizer)
    {

        RuleFor(u => u.NameAr)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (req, name, _) => !await repository.AnyAsync(new ExpressionSpecification<NutrientsAttribute>(x => x.NameAr == name && req.Id != x.Id), _))
                 .WithMessage(localaizer["Arabic name already exists"]);

        RuleFor(u => u.NameEn)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (req, name, _) => !await repository.AnyAsync(new ExpressionSpecification<NutrientsAttribute>(x => x.NameEn == name && req.Id != x.Id), _))
                .WithMessage(localaizer["English name already exists"]);

    }
}

public class UpdateNutrientsAttributeRequestHandler(
    IRepositoryWithEvents<NutrientsAttribute> repository,
    IFileStorageManager fileStorageManager,
    IStringLocalizer<UpdateNutrientsAttributeRequestHandler> localizer) : IRequestHandler<UpdateNutrientsAttributeRequest, Result<Guid>>
{
    private readonly IRepositoryWithEvents<NutrientsAttribute> _repository = repository;
    private readonly IStringLocalizer<UpdateNutrientsAttributeRequestHandler> _localizer = localizer;
    private readonly IFileStorageManager _fileStorageManager = fileStorageManager;

    public async Task<Result<Guid>> Handle(UpdateNutrientsAttributeRequest request, CancellationToken cancellationToken)
    {
        var part = await _repository.GetByIdAsync(request.Id!.Value, cancellationToken);

        _ = part ?? throw new NotFoundException(_localizer["NutrientsAttribute not found"]);

        part.NameAr = request.NameAr;
        part.NameEn = request.NameEn;
        part.Unit = request.Unit;
        // part.IconUrl = request.Icon == null ? part.IconUrl : await _fileStorageManager.UploadAsync<NutrientsAttribute>(request.Icon, FileType.Image, ModuleConstant.ModuleName, cancellationToken);

        await _repository.UpdateAsync(part, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<Guid>.SuccessAsync(part.Id);
    }
}

