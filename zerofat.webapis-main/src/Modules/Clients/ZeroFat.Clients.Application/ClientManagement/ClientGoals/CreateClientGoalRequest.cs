using MediatR;
using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Validation;
using ZeroFat.Application.Core.Contracts;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Domain.ClientManagement;
using ZeroFat.ClientPortal.Domain.NutritionTracking;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.ClientManagement.ClientGoals;
public class CreateClientGoalRequest : ICommand<Result<DefaultIdType>>
{
    public Gender Gender { get; set; }
    public DietitianGoal DietitianGoal { get; set; }
    public DateTime BirthDate { get; set; }

    public double HeightInCM { get; set; }
    public double WeightInKG { get; set; }
    public double TargetWeightInKG { get; set; }
    public double ActivityValue { get; set; }

    public Guid PhysicalActivityLevelId { get; set; }

    public HeightMeasurement HeightMeasurement { get; set; }
    public WeightMeasurement WeightMeasurement { get; set; }
    public WeightMeasurement TargetWeightMeasurement { get; set; }
}

public class CreateClientGoalRequestValidator : CustomValidator<CreateClientGoalRequest>
{
    public CreateClientGoalRequestValidator(
        IReadRepository<ClientGoal> repository,
        IStringLocalizer<CreateClientGoalRequestValidator> localizer)
    {
        RuleFor(x => x.Gender)
            .IsInEnum()
            .WithMessage(localizer["Gender must be a valid value."]);

        RuleFor(x => x.DietitianGoal)
            .IsInEnum()
            .WithMessage(localizer["DietitianGoal must be a valid value."]);

        RuleFor(x => x.BirthDate)
            .NotEmpty()
            .WithMessage(localizer["BirthDate is required."])
            .Must(BeAValidDate)
            .WithMessage(localizer["BirthDate must be a valid date."])
            .Must(BeAtLeast12YearsOld)
            .WithMessage(localizer["Client must be at least 12 years old."]);

        RuleFor(x => x.HeightInCM)
            .GreaterThan(0)
            .WithMessage(localizer["Height must be greater than 0."])
            .LessThanOrEqualTo(300)
            .WithMessage(localizer["Height must be less than or equal to 300 cm."]);

        RuleFor(x => x.WeightInKG)
            .GreaterThan(0)
            .WithMessage(localizer["Weight must be greater than 0."])
            .LessThanOrEqualTo(500)
            .WithMessage(localizer["Weight must be less than or equal to 500 kg."]);

        RuleFor(x => x.TargetWeightInKG)
            .GreaterThan(0)
            .WithMessage(localizer["Target weight must be greater than 0."])
            .LessThanOrEqualTo(500)
            .WithMessage(localizer["Target weight must be less than or equal to 500 kg."]);

        RuleFor(x => x.ActivityValue)
            .GreaterThan(0)
            .WithMessage(localizer["Activity value must be greater than 0."])
            .LessThanOrEqualTo(3)
            .WithMessage(localizer["Activity value must be less than or equal to 3."]);

        // RuleFor(x => x.PhysicalActivityLevelId)
        //     .NotEmpty()
        //     .WithMessage(localizer["PhysicalActivityLevelId is required."])
        //     .Must(BeAValidGuid)
        //     .WithMessage(localizer["PhysicalActivityLevelId must be a valid GUID."]);

        RuleFor(x => x.HeightMeasurement)
            .IsInEnum()
            .WithMessage(localizer["HeightMeasurement must be a valid value."]);

        RuleFor(x => x.WeightMeasurement)
            .IsInEnum()
            .WithMessage(localizer["WeightMeasurement must be a valid value."]);

        RuleFor(x => x.TargetWeightMeasurement)
            .IsInEnum()
            .WithMessage(localizer["TargetWeightMeasurement must be a valid value."]);
    }

    private static bool BeAValidDate(DateTime date)
    {
        return date != default && date <= DateTime.Now;
    }

    private static bool BeAtLeast12YearsOld(DateTime birthDate)
    {
        var age = DateTime.Now.Year - birthDate.Year;
        if (birthDate > DateTime.Now.AddYears(-age))
        {
            age--;
        }
        return age >= 12;
    }

}


public class CreateClientGoalRequestHandler(
    IRepositoryWithEvents<ClientGoal> repo,
    ICurrentUser currentUser,
    IZeroFatService zeroFatService,
    IClientPortalSettingservice clientPortalSettingservice,
    IRepositoryWithEvents<Client> clientRepo) : IRequestHandler<CreateClientGoalRequest, Result<DefaultIdType>>
{
    private readonly IRepositoryWithEvents<ClientGoal> _repo = repo;
    private readonly IRepositoryWithEvents<Client> _clientRepo = clientRepo;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IZeroFatService _zeroFatService = zeroFatService;
    private readonly IClientPortalSettingservice _clientPortalSettingservice = clientPortalSettingservice;



    public async Task<Result<DefaultIdType>> Handle(CreateClientGoalRequest request, CancellationToken cancellationToken)
    {
        var client = await _clientRepo.GetByIdAsync(_currentUser.GetUserId(), cancellationToken);
        _ = client ?? throw new NotFoundException("Client not found");

        var bMI = _zeroFatService.CalculateClientBMI(request.WeightInKG, request.HeightInCM);
        var bMR = _zeroFatService.CalculateClientBMR(request.WeightInKG, request.HeightInCM, request.BirthDate, request.Gender);
        var bodyFat = _zeroFatService.CalculateBodyFatBasedOnBMI(bMI, request.BirthDate, request.Gender);
        var tDEE = _zeroFatService.CalculateClientTDEE(bMR, request.ActivityValue);
        var usedStrategy = await _clientPortalSettingservice.GetNutriPlanStartegy();
        int timeInWeeks;
        double neededCalories;
        if (usedStrategy == NutriPlanStartegy.BasedOnCalories)
        {
            var deficit = await _clientPortalSettingservice.GetWeeklyCaloricDeficit();
            var surplus = await _clientPortalSettingservice.GetWeeklyCaloricSurplus();
            (timeInWeeks, neededCalories) = _zeroFatService.CalculateClientDailyCalories(
                request.WeightInKG,
                request.TargetWeightInKG,
                tDEE,
                usedStrategy,
                deficit,
                surplus);
        }
        else
        {
            timeInWeeks = await _clientPortalSettingservice.GetDefaultNutriPlanTimeAvailable();
            (timeInWeeks, neededCalories) = _zeroFatService.CalculateClientDailyCalories(
                request.WeightInKG,
                request.TargetWeightInKG,
                tDEE,
                usedStrategy,
                timeInWeeks: timeInWeeks);
        }

        client.BirthDate = DateTime.SpecifyKind(request.BirthDate, DateTimeKind.Utc);
        client.ActivityValue = request.ActivityValue;
        client.DietitianGoal = request.DietitianGoal;
        client.HeightInCM = request.HeightInCM;
        client.WeightInKG = request.WeightInKG;
        client.CurrentWeightInKG = request.WeightInKG;
        client.Gender = request.Gender;
        client.TargetWeightInKG = request.TargetWeightInKG;
        client.BMI = bMI;
        client.BMR = bMR;
        client.BodyFat = bodyFat;
        client.TDEE = tDEE;
        client.TimeToReachGoalInDays = timeInWeeks * 7;
        client.NeededCaloriesToReachGoal = neededCalories;
        client.NewGoalStart = DateOnly.FromDateTime(DateTime.Now);
        await _repo.AddAsync(new ClientGoal()
        {
            HeightInCM = request.HeightInCM,
            WeightInKG = request.WeightInKG,
            TargetWeightInKG = request.TargetWeightInKG,
            DietitianGoal = request.DietitianGoal,
            PhysicalActivityLevelId = request.PhysicalActivityLevelId,
            TargetWeightMeasurement = request.TargetWeightMeasurement,
            WeightMeasurement = request.WeightMeasurement,
            HeightMeasurement = request.HeightMeasurement,
            ActivityValue = request.ActivityValue,
            BMI = bMI,
            BMR = bMR,
            BodyFat = bodyFat,
            TDEE = tDEE,
            TimeToReachGoalInDays = timeInWeeks * 7,
            NeededCaloriesToReachGoal = neededCalories,
            ClientId = _currentUser.GetUserId()
        },
        withSaveChanges: false,
        cancellationToken: cancellationToken);

        await _clientRepo.UpdateAsync(client, withSaveChanges: true, cancellationToken: cancellationToken);

        return await Result<DefaultIdType>.SuccessAsync(client.Id);
    }

}
