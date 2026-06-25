using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;
using ZeroFat.GymUp.Domain;

namespace ZeroFat.GymUp.Application.Catalog.WorkoutTypes;
public class CreateWorkoutTypeRequest : ICommand<Result<Guid>>
{
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public IFormFile? Icon { get; set; }
    public bool? IsActive { get; set; }
}

public class CreateWorkoutTypeRequestValidator : CustomValidator<CreateWorkoutTypeRequest>
{
    public CreateWorkoutTypeRequestValidator(IReadRepository<WorkoutType> repository, IStringLocalizer<CreateWorkoutTypeRequestValidator> localaizer)
    {

        RuleFor(u => u.NameAr)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (name, _) => !await repository.AnyAsync(new ExpressionSpecification<WorkoutType>(x => x.NameAr == name), _))
                 .WithMessage(localaizer["Arabic name already exists"]);

        RuleFor(u => u.NameEn)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (name, _) => !await repository.AnyAsync(new ExpressionSpecification<WorkoutType>(x => x.NameEn == name), _))
                .WithMessage(localaizer["English name already exists"]);

    }
}


public class CreateWorkoutTypeRequestHandler(IRepositoryWithEvents<WorkoutType> repository, IFileStorageManager uploadFile) : IRequestHandler<CreateWorkoutTypeRequest, Result<Guid>>
{
    private readonly IRepositoryWithEvents<WorkoutType> _repository = repository;
    private readonly IFileStorageManager _uploadFile = uploadFile;

    public async Task<Result<Guid>> Handle(CreateWorkoutTypeRequest request, CancellationToken cancellationToken)
    {
        var cate = new WorkoutType
        {
            NameEn = request.NameEn,
            NameAr = request.NameAr,
            IconUrl = await _uploadFile.UploadAsync<WorkoutType>(request.Icon, FileType.Image, ModuleConstant.ModuleName, cancellationToken),
            IsActive = request.IsActive ?? false
        };

        await _repository.AddAsync(cate, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<Guid>.SuccessAsync(cate.Id);
    }

}
