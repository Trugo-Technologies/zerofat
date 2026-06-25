using ZeroFat.Domain.Common.Contracts;

namespace ZeroFat.Application.Common.Models;

public class BaseEntityActivationDetailsDto : BaseEntityAuditableDetailsDto, IHaveActivation
{
    public bool IsActive { get; set; }
    public DateTime? ActivationChangedOn { get; set; }
    public string? ActivationChangedByName { get; set; }
    public DefaultIdType? ActivationChangedBy { get; set; }
}
