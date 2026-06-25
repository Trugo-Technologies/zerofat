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
using ZeroFat.GymUp.Domain.Catalog;

namespace ZeroFat.GymUp.Application.Catalog.PlanGoals;
public class UpdatePlanGoalRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType? Id { get; set; }
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public IFormFile? Image { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
}

public class UpdatePlanGoalRequestValidator : CustomValidator<UpdatePlanGoalRequest>
{
    public UpdatePlanGoalRequestValidator(IReadRepository<PlanGoal> repository, IStringLocalizer<UpdatePlanGoalRequestValidator> localaizer)
    {

        RuleFor(u => u.NameAr)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (req, name, _) => !await repository.AnyAsync(new ExpressionSpecification<PlanGoal>(x => x.NameAr == name && req.Id != x.Id), _))
                 .WithMessage(localaizer["Arabic name already exists"]);

        RuleFor(u => u.NameEn)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (req, name, _) => !await repository.AnyAsync(new ExpressionSpecification<PlanGoal>(x => x.NameEn == name && req.Id != x.Id), _))
                .WithMessage(localaizer["English name already exists"]);

    }
}

public class UpdatePlanGoalRequestHandler(IRepositoryWithEvents<PlanGoal> repository, IStringLocalizer<UpdatePlanGoalRequestHandler> localizer, IFileStorageManager uploadFile) : IRequestHandler<UpdatePlanGoalRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<PlanGoal> _repository = repository;
    private readonly IStringLocalizer<UpdatePlanGoalRequestHandler> _localizer = localizer;
    private readonly IFileStorageManager _uploadFile = uploadFile;


    public async Task<Result<DefaultIdType>> Handle(UpdatePlanGoalRequest request, CancellationToken cancellationToken)
    {
        var goal = await _repository.GetByIdAsync(request.Id!.Value, cancellationToken);

        _ = goal ?? throw new NotFoundException(_localizer["Category not found"]);

        goal.NameAr = request.NameAr;
        goal.NameEn = request.NameEn;
        goal.IsActive = request.IsActive;

        if (request.Image != null)
        {
            goal.ImageUrl = await _uploadFile.UploadAsync<PlanGoal>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            goal.ImageThumbnailUrl = await _uploadFile.UploadThumbnailAsync<PlanGoal>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            goal.ImageOptimzeUrl = await _uploadFile.UploadNormalAsync<PlanGoal>(request.Image, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
        }

        await _repository.UpdateAsync(goal, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(goal.Id);
    }
}

