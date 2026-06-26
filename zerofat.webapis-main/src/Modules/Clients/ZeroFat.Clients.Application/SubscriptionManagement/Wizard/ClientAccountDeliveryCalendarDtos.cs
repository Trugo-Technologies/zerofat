using ZeroFat.Application.Common.Interfaces;
using ZeroFat.ClientPortal.Domain.SubscriptionManagement;
using ZeroFat.Domain.Enums;

namespace ZeroFat.ClientPortal.Application.SubscriptionManagement.Wizard;

public enum ClientDeliveryCalendarDayStatus
{
    Pending,
    Delivered,
    Skipped,
    Moved,
    Unavailable
}

public enum DeliveryAdjustmentScope
{
    ThisDayOnly,
    AllUpcomingDays,
    EntireOrder,
    SpecificMealsOnly
}

public class ClientDeliveryCalendarResultDto : IDto
{
    public int Month { get; set; }
    public int Year { get; set; }
    public ClientDeliveryCalendarSummaryDto Summary { get; set; } = new();
    public List<ClientDeliveryCalendarDayDto> Days { get; set; } = [];
}

public class ClientDeliveryCalendarSummaryDto : IDto
{
    public int Delivered { get; set; }
    public int Pending { get; set; }
    public int Skipped { get; set; }
    public int Moved { get; set; }
}

public class ClientDeliveryCalendarDayDto : IDto
{
    public DateOnly Date { get; set; }
    public ClientDeliveryCalendarDayStatus Status { get; set; }
    public int MealCount { get; set; }
    public DefaultIdType? DailySelectionId { get; set; }
}

public class ClientDeliveryDayDetailDto : IDto
{
    public DateOnly Date { get; set; }
    public ClientDeliveryCalendarDayStatus Status { get; set; }
    public string? DeliveryPaymentMethod { get; set; }
    public string? ScheduleLabel { get; set; }
    public string? ActiveMealPlanName { get; set; }
    public decimal? AverageCalories { get; set; }
    public DateOnly? ReplacementDate { get; set; }
    public DefaultIdType? DailySelectionId { get; set; }
    public List<ClientDeliveryDayMealDto> Meals { get; set; } = [];
    public ClientDeliveryCutoffSettingsDto CutoffSettings { get; set; } = new();
    public ClientDeliveryPaymentResultDto? LastPayment { get; set; }
}

public class ClientDeliveryDayMealDto : IDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType? MealTypeId { get; set; }
    public string? MealTypeName { get; set; }
    public string? MealName { get; set; }
    public MealSelectionType MealSelectionType { get; set; }
    public DefaultIdType? MealId { get; set; }
    public int Qty { get; set; }
    public decimal BasePrice { get; set; }
    public decimal? AdjustedPrice { get; set; }
    public decimal LineTotal { get; set; }
    public bool IsPaid { get; set; }
}

public class ClientDeliveryCutoffSettingsDto : IDto
{
    public bool EnableCutoffRestriction { get; set; }
    public int CutoffValue { get; set; }
    public string CutoffUnit { get; set; } = "Hours";
}

public class ClientDeliveryPaymentResultDto : IDto
{
    public decimal AmountDue { get; set; }
    public decimal AmountCharged { get; set; }
    public bool RequiresPayment { get; set; }
    public bool PaymentWaived { get; set; }
    public DefaultIdType? PaymentId { get; set; }
}

public class ClientDeliveryAddOnItemDto : IDto
{
    public DefaultIdType MealId { get; set; }
    public int Quantity { get; set; } = 1;
}

public class ClientDeliveryMealReplacementDto : IDto
{
    public DefaultIdType DailyMealSelectionId { get; set; }
    public DefaultIdType MealId { get; set; }
    public DefaultIdType DailyMenuMealId { get; set; }
}

public class ClientDeliveryMealPlanSlotDto : IDto
{
    public DefaultIdType MealTypeId { get; set; }
    public bool Enabled { get; set; }
    public int QuantityPerDay { get; set; } = 1;
}

public class ClientDeliveryAddOnPreviewDto : IDto
{
    public DateOnly Date { get; set; }
    public List<ClientDeliveryAddOnPreviewLineDto> Lines { get; set; } = [];
    public decimal TotalPrice { get; set; }
    public bool RequiresPayment { get; set; }
}

public class ClientDeliveryAddOnPreviewLineDto : IDto
{
    public DefaultIdType MealId { get; set; }
    public string? MealName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}

public class ClientDeliveryReplaceMealsPreviewDto : IDto
{
    public DateOnly Date { get; set; }
    public List<ClientDeliveryReplaceMealPreviewLineDto> Lines { get; set; } = [];
    public decimal TotalUpgradeCost { get; set; }
    public bool RequiresPayment { get; set; }
}

public class ClientDeliveryReplaceMealPreviewLineDto : IDto
{
    public DefaultIdType DailyMealSelectionId { get; set; }
    public string? MealTypeName { get; set; }
    public string? CurrentMealName { get; set; }
    public string? NewMealName { get; set; }
    public decimal BasePrice { get; set; }
    public decimal NewMenuPrice { get; set; }
    public decimal UpgradeCost { get; set; }
}

public class ClientDeliveryMealPlanChangePreviewDto : IDto
{
    public DateOnly Date { get; set; }
    public decimal CurrentPlanTotal { get; set; }
    public decimal NewPlanTotal { get; set; }
    public decimal PeriodDifference { get; set; }
    public decimal ProratedChargeAmount { get; set; }
    public bool RequiresPayment { get; set; }
    public bool ApplyStartingFromThisDay { get; set; }
}
