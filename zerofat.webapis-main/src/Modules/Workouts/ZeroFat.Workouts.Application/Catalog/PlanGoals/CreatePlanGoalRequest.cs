using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;
using ZeroFat.GymUp.Domain;

namespace ZeroFat.GymUp.Application.Catalog.PlanGoals;
public class CreatePlanGoalRequest : ICommand<Result<DefaultIdType>>
{
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public IFormFile? Image { get; set; }
    public bool? IsActive { get; set; }
}

public class CreatePlanGoalRequestValidator : CustomValidator<CreatePlanGoalRequest>
{
    public CreatePlanGoalRequestValidator(IReadRepository<PlanGoal> repository, IStringLocalizer<CreatePlanGoalRequestValidator> localaizer)
    {

        RuleFor(u => u.NameAr)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (name, _) => !await repository.AnyAsync(new ExpressionSpecification<PlanGoal>(x => x.NameAr == name), _))
                 .WithMessage(localaizer["Arabic name already exists"]);

        RuleFor(u => u.NameEn)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (name, _) => !await repository.AnyAsync(new ExpressionSpecification<PlanGoal>(x => x.NameEn == name), _))
                .WithMessage(localaizer["English name already exists"]);

    }
}


public class CreatePlanGoalRequestHandler(IRepositoryWithEvents<PlanGoal> repository, IFileStorageManager uploadFile) : IRequestHandler<CreatePlanGoalRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<PlanGoal> _repository = repository;
    private readonly IFileStorageManager _uploadFile = uploadFile;

    public async Task<Result<DefaultIdType>> Handle(CreatePlanGoalRequest request, CancellationToken cancellationToken)
    {
        var planGoal = new PlanGoal
        {
            NameEn = request.NameEn,
            NameAr = request.NameAr,
            IsActive = request.IsActive ?? false
        };

        if (request.Image != null)
        {
            planGoal.ImageUrl = await _uploadFile.UploadAsync<PlanGoal>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            planGoal.ImageThumbnailUrl = await _uploadFile.UploadThumbnailAsync<PlanGoal>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            planGoal.ImageOptimzeUrl = await _uploadFile.UploadNormalAsync<PlanGoal>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
        }

        await _repository.AddAsync(planGoal, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(planGoal.Id);
    }

}
