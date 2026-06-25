namespace ZeroFat.Domain.Common.Contracts;

public interface IHaveActivation
{
    public bool IsActive { get; set; }
    public DateTime? ActivationChangedOn { get; set; }
    public string? ActivationChangedByName { get; set; }
    public DefaultIdType? ActivationChangedBy { get; set; }
}
