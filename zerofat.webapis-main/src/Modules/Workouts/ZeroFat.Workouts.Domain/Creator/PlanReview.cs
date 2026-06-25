using System.ComponentModel.DataAnnotations.Schema;
using ZeroFat.Domain.Enums;

namespace ZeroFat.GymUp.Domain.Creator;

public class PlanReview : AuditableEntity, IAggregateRoot
{
    public DefaultIdType PlanId { get; set; }
    public DefaultIdType UserId { get; set; }
    public double EffectivenessRate { get; set; }
    public double EasyToUseRate { get; set; }
    public double EnjoyabilityRate { get; set; }
    public double TotalRate { get; set; }
    public string? Content { get; set; }
    public virtual Plan? Plan { get; set; }
    [ForeignKey("UserId")]
    public virtual Client? Client { get; set; }

}

public class Client : Entity, IAggregateRoot
{
    public string? FullName { get; set; }
    public string? ImageUrl { get; set; }
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public Gender? Gender { get; set; }
    public DateTime? BirthDate { get; set; }
}
