using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;
using ZeroFat.GymUp.Domain;

namespace ZeroFat.GymUp.Application.Catalog.WorkoutTypes;
public class UpdateWorkoutTypeRequest : ICommand<Result<Guid>>
{
    public Guid? Id { get; set; }
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public IFormFile? Icon { get; set; }
    public string? IconUrl { get; set; }
    public bool IsActive { get; set; }
}

public class UpdateWorkoutTypeRequestValidator : CustomValidator<UpdateWorkoutTypeRequest>
{
    public UpdateWorkoutTypeRequestValidator(IReadRepository<WorkoutType> repository, IStringLocalizer<UpdateWorkoutTypeRequestValidator> localaizer)
    {

        RuleFor(u => u.NameAr)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (req, name, _) => !await repository.AnyAsync(new ExpressionSpecification<WorkoutType>(x => x.NameAr == name && req.Id != x.Id), _))
                 .WithMessage(localaizer["Arabic name already exists"]);

        RuleFor(u => u.NameEn)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (req, name, _) => !await repository.AnyAsync(new ExpressionSpecification<WorkoutType>(x => x.NameEn == name && req.Id != x.Id), _))
                .WithMessage(localaizer["English name already exists"]);

    }
}

public class UpdateWorkoutTypeRequestHandler(IRepositoryWithEvents<WorkoutType> repository, IStringLocalizer<UpdateWorkoutTypeRequestHandler> localizer, IFileStorageManager uploadFile) : IRequestHandler<UpdateWorkoutTypeRequest, Result<Guid>>
{
    private readonly IRepositoryWithEvents<WorkoutType> _repository = repository;
    private readonly IStringLocalizer<UpdateWorkoutTypeRequestHandler> _localizer = localizer;
    private readonly IFileStorageManager _uploadFile = uploadFile;


    public async Task<Result<Guid>> Handle(UpdateWorkoutTypeRequest request, CancellationToken cancellationToken)
    {
        var cate = await _repository.GetByIdAsync(request.Id!.Value, cancellationToken);

        _ = cate ?? throw new NotFoundException(_localizer["Category not found"]);

        cate.NameAr = request.NameAr;
        cate.NameEn = request.NameEn;
        cate.IsActive = request.IsActive;
        cate.IconUrl = request.Icon != null ? await _uploadFile.UploadAsync<WorkoutType>(request.Icon, FileType.Image, ModuleConstant.ModuleName, cancellationToken) : request.IconUrl == cate.IconUrl ? cate.IconUrl : null;


        await _repository.UpdateAsync(cate, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<Guid>.SuccessAsync(cate.Id);
    }
}

