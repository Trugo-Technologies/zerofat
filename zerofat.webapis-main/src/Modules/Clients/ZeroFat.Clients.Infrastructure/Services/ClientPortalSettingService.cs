using Microsoft.EntityFrameworkCore;
using ZeroFat.Application.Common.Exceptions;
using ZeroFat.Infrastructure.Core.Persistence.Context;
using Ardalis.Specification.EntityFrameworkCore;
using ZeroFat.ClientPortal.Application.Contracts;
using ZeroFat.ClientPortal.Application.Settings.ClientPortalSettings;
using ZeroFat.Shared.Constans;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Infrastructure.Services;

public class ClientPortalSettingService : IClientPortalSettingservice
{
    private readonly CoreContext _context;
    public ClientPortalSettingService(CoreContext context) => _context = context;

    #region GetServices
    public async Task<int> GetOffsetSubscriptionInDays()
    {
        var p = await _context.Settings
                                .WithSpecification(new ClientPortalSettingByNameSpec(ClientPortalSettingConstants.OffsetSubscriptionInDays))
                                .FirstOrDefaultAsync();

        _ = p ?? throw new NotFoundException("offsetSubscriptionInDays is not found");

        return int.Parse(p.Value);
    }

    public async Task<int> GetDefaultNutriPlanTimeAvailable()
    {
        var p = await _context.Settings
                               .WithSpecification(new ClientPortalSettingByNameSpec(ClientPortalSettingConstants.DefaultNutriPlanTimeAvailable))
                               .FirstOrDefaultAsync();

        _ = p ?? throw new NotFoundException("DefaultNutriPlanTimeAvailable is not found");

        return int.Parse(p.Value);
    }

    public async Task<NutriPlanStartegy> GetNutriPlanStartegy()
    {
        var p = await _context.Settings
                                .WithSpecification(new ClientPortalSettingByNameSpec(ClientPortalSettingConstants.NutriPlanBasedStartegy))
                                .FirstOrDefaultAsync();

        _ = p ?? throw new NotFoundException("NutriPlanBasedStartegy is not found");

        return (NutriPlanStartegy)Enum.Parse(typeof(NutriPlanStartegy), p.Value);
    }
    public async Task<int> GetWeeklyCaloricDeficit()
    {
        var p = await _context.Settings
                               .WithSpecification(new ClientPortalSettingByNameSpec(ClientPortalSettingConstants.WeeklyCaloricDeficit))
                               .FirstOrDefaultAsync();

        _ = p ?? throw new NotFoundException("WeeklyCaloricDeficit is not found");

        return int.Parse(p.Value);
    }

    public async Task<int> GetOnePointEqualInMoney()
    {
        var p = await _context.Settings
                               .WithSpecification(new ClientPortalSettingByNameSpec(ClientPortalSettingConstants.OnePointEqualInMoney))
                               .FirstOrDefaultAsync();

        _ = p ?? throw new NotFoundException("OnePointEqualInMoney is not found");

        return int.Parse(p.Value);
    }

    public async Task<int> GetWeeklyCaloricSurplus()
    {
        var p = await _context.Settings
                               .WithSpecification(new ClientPortalSettingByNameSpec(ClientPortalSettingConstants.WeeklyCaloricSurplus))
                               .FirstOrDefaultAsync();

        _ = p ?? throw new NotFoundException("WeeklyCaloricSurplus is not found");

        return int.Parse(p.Value);
    }

    public async Task<int> GetMonthlyPackageSubsciptionDiscount()
    {
        var p = await _context.Settings
                                .WithSpecification(new ClientPortalSettingByNameSpec(ClientPortalSettingConstants.MonthlyPackageSubsciptionDiscount))
                                .FirstOrDefaultAsync();

        _ = p ?? throw new NotFoundException("MonthlyPackageSubsciptionDiscount is not found");

        return int.Parse(p.Value);
    }

    public async Task<int> GetThreeMonthlyPackageSubsciptionDiscount()
    {
        var p = await _context.Settings
                                .WithSpecification(new ClientPortalSettingByNameSpec(ClientPortalSettingConstants.ThreeMonthlyPackageSubsciptionDiscount))
                                .FirstOrDefaultAsync();

        _ = p ?? throw new NotFoundException("ThreeMonthlyPackageSubsciptionDiscount is not found");

        return int.Parse(p.Value);
    }

    public async Task<int> GetMinimumSubsciption()
    {
        var p = await _context.Settings
                                .WithSpecification(new ClientPortalSettingByNameSpec(ClientPortalSettingConstants.MinimumSubsciption))
                                .FirstOrDefaultAsync();

        _ = p ?? throw new NotFoundException("MinimumSubsciption is not found");

        return int.Parse(p.Value);
    }

    public async Task<bool> GetEnableDailyMealSelections()
    {
        var p = await _context.Settings
                        .WithSpecification(new ClientPortalSettingByNameSpec(ClientPortalSettingConstants.EnableDailyMealSelections))
                        .FirstOrDefaultAsync();
        _ = p ?? throw new NotFoundException("enableDailyMealSelections is not found");

        return bool.Parse(p.Value);
    }
    #endregion

    #region SetServices
    public async Task SetOffsetSubscriptionInDays(int offsetSubscriptionInDays)
    {
        var p = await _context.Settings
                        .WithSpecification(new ClientPortalSettingByNameSpec(ClientPortalSettingConstants.OffsetSubscriptionInDays))
                        .FirstOrDefaultAsync();
        _ = p ?? throw new NotFoundException("offsetSubscriptionInDays is not found");

        p.Value = offsetSubscriptionInDays.ToString();
        await _context.SaveChangesAsync();
    }

    public async Task SetDefaultNutriPlanTimeAvailable(int defaultNutriPlanTimeAvailable)
    {
        var p = await _context.Settings
                        .WithSpecification(new ClientPortalSettingByNameSpec(ClientPortalSettingConstants.DefaultNutriPlanTimeAvailable))
                        .FirstOrDefaultAsync();
        _ = p ?? throw new NotFoundException("offsetSubscriptionInDays is not found");

        p.Value = defaultNutriPlanTimeAvailable.ToString();
        await _context.SaveChangesAsync();
    }

    public async Task SetNutriPlanStartegy(NutriPlanStartegy nutriPlanStartegy)
    {
        var p = await _context.Settings
                        .WithSpecification(new ClientPortalSettingByNameSpec(ClientPortalSettingConstants.NutriPlanBasedStartegy))
                        .FirstOrDefaultAsync();
        _ = p ?? throw new NotFoundException("offsetSubscriptionInDays is not found");

        p.Value = nutriPlanStartegy.ToString();
        await _context.SaveChangesAsync();
    }
    public async Task SetWeeklyCaloricDeficit(int weeklyCaloricDeficit)
    {
        var p = await _context.Settings
                        .WithSpecification(new ClientPortalSettingByNameSpec(ClientPortalSettingConstants.WeeklyCaloricDeficit))
                        .FirstOrDefaultAsync();
        _ = p ?? throw new NotFoundException("offsetSubscriptionInDays is not found");

        p.Value = weeklyCaloricDeficit.ToString();
        await _context.SaveChangesAsync();
    }

    public async Task SetWeeklyCaloricSurplus(int weeklyCaloricSurplus)
    {
        var p = await _context.Settings
                        .WithSpecification(new ClientPortalSettingByNameSpec(ClientPortalSettingConstants.WeeklyCaloricSurplus))
                        .FirstOrDefaultAsync();
        _ = p ?? throw new NotFoundException("offsetSubscriptionInDays is not found");

        p.Value = weeklyCaloricSurplus.ToString();
        await _context.SaveChangesAsync();
    }

    public async Task SetMonthlyPackageSubsciptionDiscount(int monthlyPackageSubsciptionDiscount)
    {
        var p = await _context.Settings
                        .WithSpecification(new ClientPortalSettingByNameSpec(ClientPortalSettingConstants.MonthlyPackageSubsciptionDiscount))
                        .FirstOrDefaultAsync();
        _ = p ?? throw new NotFoundException("monthlyPackageSubsciptionDiscount is not found");

        p.Value = monthlyPackageSubsciptionDiscount.ToString();
        await _context.SaveChangesAsync();
    }

    public async Task SetThreeMonthlyPackageSubsciptionDiscount(int threeMonthlyPackageSubsciptionDiscount)
    {
        var p = await _context.Settings
                        .WithSpecification(new ClientPortalSettingByNameSpec(ClientPortalSettingConstants.ThreeMonthlyPackageSubsciptionDiscount))
                        .FirstOrDefaultAsync();
        _ = p ?? throw new NotFoundException("threeMonthlyPackageSubsciptionDiscount is not found");

        p.Value = threeMonthlyPackageSubsciptionDiscount.ToString();
        await _context.SaveChangesAsync();
    }
    
    public async Task SetMinimumSubsciption(int minimumSubsciption)
    {
        var p = await _context.Settings
                        .WithSpecification(new ClientPortalSettingByNameSpec(ClientPortalSettingConstants.MinimumSubsciption))
                        .FirstOrDefaultAsync();
        _ = p ?? throw new NotFoundException("minimumSubsciption is not found");

        p.Value = minimumSubsciption.ToString();
        await _context.SaveChangesAsync();
    }

    public async Task SetOnePointEqualInMoney(int onePointEqualInMoney)
    {
        var p = await _context.Settings
                       .WithSpecification(new ClientPortalSettingByNameSpec(ClientPortalSettingConstants.OnePointEqualInMoney))
                       .FirstOrDefaultAsync();
        _ = p ?? throw new NotFoundException("onePointEqualInMoney is not found");

        p.Value = onePointEqualInMoney.ToString();
        await _context.SaveChangesAsync();
    }
   
    public async Task SetEnableDailyMealSelections(bool value)
    {
        var p = await _context.Settings
                        .WithSpecification(new ClientPortalSettingByNameSpec(ClientPortalSettingConstants.EnableDailyMealSelections))
                        .FirstOrDefaultAsync();
        _ = p ?? throw new NotFoundException("enableDailyMealSelections is not found");

        p.Value = value.ToString();
        await _context.SaveChangesAsync();
    }
    #endregion
}
