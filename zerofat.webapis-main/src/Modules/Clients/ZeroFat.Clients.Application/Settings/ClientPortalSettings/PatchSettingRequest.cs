using Microsoft.Extensions.Localization;
using ZeroFat.Application.Common.Persistence;
using ZeroFat.Application.Common.Validation;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.Domain.Core;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.Settings.ClientPortalSettings;
public class PatchSettingRequest : ICommand<Result>
{
    public int? OffsetSubscriptionInDays { get; set; }
    public NutriPlanStartegy? NutriPlanStartegy { get; set; }
    public int? WeeklyCaloricDeficit { get; set; }
    public int? WeeklyCaloricSurplus { get; set; }
    public int? DefaultNutriPlanTimeAvailable { get; set; }

    public int? MonthlyPackageSubsciptionDiscount { get; set; }
    public int? MinimumSubsciption { get; set; }
    public int? ThreeMonthlyPackageSubsciptionDiscount { get; set; }
    public int? OnePointEqualInMoney { get; set; }
    public bool? EnableDailyMealSelections { get; set; }
}
internal sealed class PatchSettingRequestValidator : CustomValidator<UpdateSettingRequest>
{
    public PatchSettingRequestValidator(
        IReadRepository<Setting> setting)
    {
    }
}
public class PatchSettingRequestHandler : ICommandHandler<PatchSettingRequest, Result>
{
    // Add Domain Events automatically by using IRepositoryWithEvents
    private readonly IClientPortalSettingservice _settingService;
    public PatchSettingRequestHandler(
        IClientPortalSettingservice settingService)
    {
        (_settingService) = (settingService);
    }

    public async Task<Result> Handle(PatchSettingRequest request, CancellationToken cancellationToken)
    {
        if (request.OffsetSubscriptionInDays != null)
        {
            await _settingService.SetOffsetSubscriptionInDays(request.OffsetSubscriptionInDays.Value);
        }

        if (request.DefaultNutriPlanTimeAvailable != null)
        {
            await _settingService.SetDefaultNutriPlanTimeAvailable(request.DefaultNutriPlanTimeAvailable.Value);
        }

        if (request.NutriPlanStartegy != null)
        {
            await _settingService.SetNutriPlanStartegy(request.NutriPlanStartegy.Value);
        }

        if (request.WeeklyCaloricDeficit != null)
        {
            await _settingService.SetWeeklyCaloricDeficit(request.WeeklyCaloricDeficit.Value);
        }

        if (request.WeeklyCaloricSurplus != null)
        {
            await _settingService.SetWeeklyCaloricSurplus(request.WeeklyCaloricSurplus.Value);
        }

        if (request.MinimumSubsciption != null)
        {
            await _settingService.SetMinimumSubsciption(request.MinimumSubsciption.Value);
        }

        if (request.ThreeMonthlyPackageSubsciptionDiscount != null)
        {
            await _settingService.SetThreeMonthlyPackageSubsciptionDiscount(request.ThreeMonthlyPackageSubsciptionDiscount.Value);
        }

        if (request.MonthlyPackageSubsciptionDiscount != null)
        {
            await _settingService.SetMonthlyPackageSubsciptionDiscount(request.MonthlyPackageSubsciptionDiscount.Value);
        }

        if (request.OnePointEqualInMoney != null)
        {
            await _settingService.SetOnePointEqualInMoney(request.OnePointEqualInMoney.Value);
        }

        if (request.EnableDailyMealSelections != null)
        {
            await _settingService.SetEnableDailyMealSelections(request.EnableDailyMealSelections.Value);
        }

        return (Result)await Result.SuccessAsync();
    }
}
