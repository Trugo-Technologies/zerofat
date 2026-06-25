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
using ZeroFat.GymUp.Domain.Creator;

namespace ZeroFat.GymUp.Application.Creator.Plans;
public class UpdatePlanRequest : ICommand<Result<DefaultIdType>>
{
    public DefaultIdType Id { get; set; }
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public IFormFile? AvatarImage { get; set; }
    public IFormFile? ProfileMedia { get; set; }
    public string? AvatarImageUrl { get; set; }
    public string? ProfileMediaUrl { get; set; }
    public int DaysPerWeek { get; set; }
    public int? WeeklyRepetition { get; set; }
    public DefaultIdType? TrainerId { get; set; }
    public DefaultIdType PlanGoalId { get; set; }
    public DefaultIdType? EquipmentCategoryId { get; set; }
    public Level? Level { get; set; }
    public GymEnvironment? Environment { get; set; }

    public string? OverviewAr { get; set; }
    public string? OverviewEn { get; set; }
    public string? PlanConclusionEn { get; set; }
    public string? PlanConclusionAr { get; set; }
    public bool IsActive { get; set; }
    public List<int> RestDays { get; set; } = [];
}

public class UpdatePlanRequestValidator : CustomValidator<UpdatePlanRequest>
{
    public UpdatePlanRequestValidator(IReadRepository<Plan> repository, IReadRepository<Trainer> trainerRepo, IReadRepository<PlanGoal> goalRepo, IStringLocalizer<UpdatePlanRequestValidator> localaizer)
    {

        RuleFor(u => u.NameAr)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MustAsync(async (req, name, _) => !await repository.AnyAsync(new ExpressionSpecification<Plan>(x => x.NameAr == name && x.TrainerId == req.TrainerId && req.Id != x.Id), _))
                 .WithMessage(localaizer["Arabic name already exists"]);

        RuleFor(u => u.NameEn)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .MustAsync(async (req, name, _) => !await repository.AnyAsync(new ExpressionSpecification<Plan>(x => x.NameEn == name && x.TrainerId == req.TrainerId && req.Id != x.Id), _))
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

        RuleFor(x => x.Level)
            .NotEmpty();

    }
}

public class UpdatePlanRequestHandler(
    IRepositoryWithEvents<Plan> repository,
    IRepositoryWithEvents<PlanSchedule> planScheduleRepo,
    IStringLocalizer<UpdatePlanRequestHandler> localizer, IFileStorageManager uploadFile) : IRequestHandler<UpdatePlanRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<Plan> _repository = repository;
    private readonly IRepositoryWithEvents<PlanSchedule> _planScheduleRepo = planScheduleRepo;
    private readonly IStringLocalizer<UpdatePlanRequestHandler> _localizer = localizer;
    private readonly IFileStorageManager _uploadFile = uploadFile;


    public async Task<Result<DefaultIdType>> Handle(UpdatePlanRequest request, CancellationToken cancellationToken)
    {
        var plan = await _repository.GetByIdAsync(request.Id, cancellationToken);

        _ = plan ?? throw new NotFoundException(_localizer["Plan not found"]);

        plan.NameAr = request.NameAr;
        plan.NameEn = request.NameEn;
        plan.DaysPerWeek = request.DaysPerWeek;
        plan.WeeklyRepetition = request.WeeklyRepetition;
        plan.TrainerId = request.TrainerId;
        plan.PlanGoalId = request.PlanGoalId;
        plan.Level = request.Level!.Value;
        plan.Environment = request.Environment;
        plan.OverviewAr = request.OverviewAr;
        plan.OverviewEn = request.OverviewEn;
        plan.PlanConclusionEn = request.PlanConclusionEn;
        plan.PlanConclusionAr = request.PlanConclusionAr;
        plan.ProfileMediaUrl = request.ProfileMedia != null ? await _uploadFile.UploadAsync<Plan>(request.ProfileMedia, FileType.Other, ModuleConstant.ModuleName, cancellationToken) : request.ProfileMediaUrl == plan.ProfileMediaUrl ? plan.ProfileMediaUrl : null;
        plan.IsActive = request.IsActive;
        plan.EquipmentCategoryId = request.EquipmentCategoryId;
        plan.RestDays = request.RestDays;

        if (request.AvatarImage != null)
        {
            plan.AvatarImageUrl = await _uploadFile.UploadAsync<Plan>(request.AvatarImage, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            plan.AvatarImageThumbnailUrl = await _uploadFile.UploadThumbnailAsync<Plan>(request.AvatarImage, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
            plan.AvatarImageOptimzeUrl = await _uploadFile.UploadNormalAsync<Plan>(request.AvatarImage, FileType.Image, ModuleConstant.ModuleName, cancellationToken);
        }

        var planShcedules = await _planScheduleRepo.ListAsync(new ExpressionSpecification<PlanSchedule>(x => x.PlanId == request.Id && (x.Day >= request.DaysPerWeek || request.RestDays.Any(day => day == x.Day))), cancellationToken);
        await _planScheduleRepo.DeleteRangeAsync(planShcedules, cancellationToken: cancellationToken);

        await _repository.UpdateAsync(plan, withSaveChanges: true, cancellationToken: cancellationToken);
        return await Result<DefaultIdType>.SuccessAsync(plan.Id);
    }
}

