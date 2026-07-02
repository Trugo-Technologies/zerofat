using Microsoft.Extensions.Localization;
using ZeroFat.ClientPortal.Application.Contracts;

namespace ZeroFat.ClientPortal.Application.Settings.ClientPortalSettings;
public class GetSettingRequest : IQuery<Result<ClientPortalSetting>>
{
}
public class GetSettingRequestHandler : IQueryHandler<GetSettingRequest, Result<ClientPortalSetting>>
{
    private readonly IStringLocalizer<GetSettingRequestHandler> _localizer;
    private readonly IClientPortalSettingservice _service;
    public GetSettingRequestHandler(IStringLocalizer<GetSettingRequestHandler> localizer, IClientPortalSettingservice service)
    {
        (_localizer, _service) = (localizer, service);
    }

    public async Task<Result<ClientPortalSetting>> Handle(GetSettingRequest request, CancellationToken cancellationToken)
    {
        var setting = new ClientPortalSetting
        {
            OffsetSubscriptionInDays = await _service.GetOffsetSubscriptionInDays(),
            CutoffTime = await _service.GetCutoffTime(),
            NutriPlanStartegy = await _service.GetNutriPlanStartegy(),
            DefaultNutriPlanTimeAvailable = await _service.GetDefaultNutriPlanTimeAvailable(),
            WeeklyCaloricDeficit = await _service.GetWeeklyCaloricDeficit(),
            WeeklyCaloricSurplus = await _service.GetWeeklyCaloricSurplus(),
            ThreeMonthlyPackageSubsciptionDiscount = await _service.GetThreeMonthlyPackageSubsciptionDiscount(),
            MonthlyPackageSubsciptionDiscount = await _service.GetMonthlyPackageSubsciptionDiscount(),
            MinimumSubsciption = await _service.GetMinimumSubsciption(),
            OnePointEqualInMoney = await _service.GetOnePointEqualInMoney(),
        };
        return await Result<ClientPortalSetting>.SuccessAsync(setting);
    }
}
