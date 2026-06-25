using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Specification;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Domain.Enums;
using ZeroFat.GymUp.Domain;
using ZeroFat.GymUp.Domain.Creator;

namespace ZeroFat.GymUp.Application.Creator.Plans;
public class CreatePlanRequest : ICommand<Result<DefaultIdType>>
{
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public IFormFile? AvatarImage { get; set; }
    public IFormFile? ProfileMedia { get; set; }
    public int DaysPerWeek { get; set; }
    public int? WeeklyRepetition { get; set; }
    public DefaultIdType? TrainerId { get; set; }
    public DefaultIdType? PlanGoalId { get; set; }
    public DefaultIdType? EquipmentCategoryId { get; set; }
    public GymEnvironment? Environment { get; set; }
    public Level? Level { get; set; }
    public string? OverviewAr { get; set; }
    public string? OverviewEn { get; set; }
    public string? PlanConclusionEn { get; set; }
    public string? PlanConclusionAr { get; set; }
    public bool? IsActive { get; set; }
    public List<int> RestDays { get; set; } = [];
}

public class CreatePlanRequestValidator : CustomValidator<CreatePlanRequest>
{
    public CreatePlanRequestValidator(IReadRepository<Plan> repository, IReadRepository<Trainer> trainerRepo, IReadRepository<PlanGoal> goalRepo, IStringLocalizer<CreatePlanRequestValidator> localaizer)
    {

        RuleFor(u => u.NameAr)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (req, name, _) => !await repository.AnyAsync(new ExpressionSpecification<Plan>(x => x.NameAr == name && x.TrainerId == req.TrainerId), _))
                 .WithMessage(localaizer["Arabic name already exists"]);

        RuleFor(u => u.NameEn)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (req, name, _) => !await repository.AnyAsync(new ExpressionSpecification<Plan>(x => x.NameEn == name && x.TrainerId == req.TrainerId), _))
                .WithMessage(localaizer["English name already exists"]);

        When(x => x.TrainerId.HasValue, () =>
        {
            RuleFor(u => u.TrainerId)
              .Cascade(CascadeMode.Stop)
              .NotEmpty()
              .MustAsync(async (id, _) => await trainerRepo.AnyAsync(new ExpressionSpecification<Trainer>(x => x.Id == id), _))
                   .WithMessage(localaizer["Trainer not found"]);

        });

        RuleFor(u => u.PlanGoalId)
          .Cascade(CascadeMode.Stop)
          .NotEmpty()
          .MustAsync(async (id, _) => await goalRepo.AnyAsync(new ExpressionSpecification<PlanGoal>(x => x.Id == id), _))
               .WithMessage(localaizer["Goal not found"]);

        RuleFor(x => x.DaysPerWeek)
            .NotEmpty()
            .GreaterThan(0)
                .WithMessage(localaizer["Days per week must be greater than 0"]);

        RuleFor(x => x.AvatarImage)
            .NotEmpty();

        RuleFor(x => x.ProfileMedia)
            .NotEmpty();

        RuleFor(x => x.Level)
            .NotEmpty();
    }
}


public class CreatePlanRequestHandler(IRepositoryWithEvents<Plan> repository, IFileStorageManager uploadFile) : IRequestHandler<CreatePlanRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<Plan> _repository = repository;
    private readonly IFileStorageManager _uploadFile = uploadFile;


    public async Task<Result<DefaultIdType>> Handle(CreatePlanRequest request, CancellationToken cancellationToken)
    {
        var plan = new Plan
        {
            NameEn = request.NameEn,
            NameAr = request.NameAr,
            DaysPerWeek = request.DaysPerWeek,
            WeeklyRepetition = request.WeeklyRepetition,
            TrainerId = request.TrainerId,
            PlanGoalId = request.PlanGoalId,
            Level = request.Level!.Value,
            OverviewAr = request.OverviewAr,
            OverviewEn = request.OverviewEn,
            PlanConclusionEn = request.PlanConclusionEn,
            PlanConclusionAr = request.PlanConclusionAr,
            Environment = request.Environment,
            RestDays = request.RestDays,
            EquipmentCategoryId = request.EquipmentCategoryId,
            ProfileMediaUrl = await _uploadFile.UploadAsync<Plan>(request.ProfileMedia, FileType.Other, ModuleConstant.ModuleName, cancellationToken),
            IsActive = request.IsActive ?? false,
        };

        if (request.AvatarImage != null)
        {
            plan.AvatarImageUrl = await _uploadFile.UploadAsync<Plan>(request.AvatarImage, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            plan.AvatarImageThumbnailUrl = await _uploadFile.UploadThumbnailAsync<Plan>(request.AvatarImage, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            plan.AvatarImageOptimzeUrl = await _uploadFile.UploadNormalAsync<Plan>(request.AvatarImage, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
        }

        await _repository.AddAsync(plan, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(plan.Id);
    }

}
