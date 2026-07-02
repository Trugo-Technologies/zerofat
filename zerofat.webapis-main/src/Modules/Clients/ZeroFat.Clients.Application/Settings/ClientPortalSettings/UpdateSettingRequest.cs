using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Validation;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.Domain.Core;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.Settings.ClientPortalSettings;
public class UpdateSettingRequest : ICommand<Result>
{
    public int OffsetSubscriptionInDays { get; set; }
    public TimeOnly? CutoffTime { get; set; }
    public NutriPlanStartegy NutriPlanStartegy { get; set; }
    public int? WeeklyCaloricDeficit { get; set; }
    public int? WeeklyCaloricSurplus { get; set; }
    public int? DefaultNutriPlanTimeAvailable { get; set; }

    public int MonthlyPackageSubsciptionDiscount { get; set; }
    public int MinimumSubsciption { get; set; }
    public int ThreeMonthlyPackageSubsciptionDiscount { get; set; }
    public int OnePointEqualInMoney { get; set; }
}
internal sealed class UpdateSettingRequestValidator : CustomValidator<UpdateSettingRequest>
{
    public UpdateSettingRequestValidator(
        IReadRepository<Setting> setting)
    {
    }
}
public class UpdateSettingRequestHandler : ICommandHandler<UpdateSettingRequest, Result>
{
    // Add Domain Events automatically by using IRepositoryWithEvents
    private readonly IRepositoryWithEvents<Setting> _settingRepo;
    private readonly IStringLocalizer<UpdateSettingRequestHandler> _localizer;
    private readonly IClientPortalSettingservice _settingService;
    public UpdateSettingRequestHandler(
        IRepositoryWithEvents<Setting> settingRepo,
        IStringLocalizer<UpdateSettingRequestHandler> localizer,
        IClientPortalSettingservice settingService
        ) => (_settingRepo, _localizer, _settingService) = (settingRepo, localizer, settingService);

    public async Task<Result> Handle(UpdateSettingRequest request, CancellationToken cancellationToken)
    {
        await _settingService.SetOffsetSubscriptionInDays(request.OffsetSubscriptionInDays);
        await _settingService.SetCutoffTime(request.CutoffTime ?? ClientPortalSetting.DefaultCutoffTime);
        await _settingService.SetDefaultNutriPlanTimeAvailable(request.DefaultNutriPlanTimeAvailable.GetValueOrDefault(0));
        await _settingService.SetNutriPlanStartegy(request.NutriPlanStartegy);

        await _settingService.SetWeeklyCaloricDeficit(request.WeeklyCaloricDeficit.GetValueOrDefault(0));
        await _settingService.SetWeeklyCaloricSurplus(request.WeeklyCaloricSurplus.GetValueOrDefault(0));
        await _settingService.SetOnePointEqualInMoney(request.OnePointEqualInMoney);

        await _settingService.SetThreeMonthlyPackageSubsciptionDiscount(request.ThreeMonthlyPackageSubsciptionDiscount);
        await _settingService.SetMonthlyPackageSubsciptionDiscount(request.MonthlyPackageSubsciptionDiscount);

        return (Result)await Result.SuccessAsync();
    }
}
