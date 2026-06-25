using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Enums;

namespace ZeroFat.GymUp.Application.Creator.PlanReviews;

public class PlanReviewSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public DefaultIdType PlanId { get; set; }
    public DefaultIdType UserId { get; set; }
    public double? EffectivenessRate { get; set; }
    public double? EasyToUseRate { get; set; }
    public double? EnjoyabilityRate { get; set; }
    public double? TotalRate { get; set; }
    public string? Content { get; set; }
    public ClientDto? Client { get; set; }
}


public class ClientDto : IDto
{
    public string? FullName { get; set; }
    public string? ImageUrl { get; set; }
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public Gender? Gender { get; set; }
    public DateTime? BirthDate { get; set; }
}
