using ZeroFat.Application.Common.Interfaces;
using ZeroFat.ClientPortal.Domain.NutritionTracking;

namespace ZeroFat.ClientPortal.Application.ClientManagement.DailyStatistics;

public class ClientDailyStatisticsSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public double? WeightInKG { get; set; }
    public double? WaterInLiter { get; set; }
    public DateOnly Date { get; set; }
}

public class ClientDailyStatisticsRawDto : ClientDailyStatisticsSimplifyDto
{
    public DefaultIdType ClientId { get; set; }
}

public class ClientDailyStatisticsAuditableDto : ClientDailyStatisticsRawDto
{
    public DateTime? CreatedOn { get; set; }
    public string? CreatedByName { get; set; }

    public string? LastModifiedByName { get; set; }
    public DateTime? LastModifiedOn { get; set; }
}

public class ClientDailyStatisticsDto : ClientDailyStatisticsAuditableDto
{
    public List<CaloriesDailyStatistics> CaloriesDailyStatistics { get; set; }
}

public class ClientDailyStatisticsDetailsDto : BaseEntityAuditableDetailsDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType ClientId { get; set; }
    public double? WeightInKG { get; set; }
    public double? WaterInLiter { get; set; }
    public DateOnly Date { get; set; }
    public List<CaloriesDailyStatistics> CaloriesDailyStatistics { get; set; }
}

