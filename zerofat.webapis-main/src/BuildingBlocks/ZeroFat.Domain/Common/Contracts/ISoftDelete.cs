namespace ZeroFat.Domain.Common.Contracts;

public interface ISoftDelete
{
    DateTime? DeletedOn { get; set; }
    DefaultIdType? DeletedBy { get; set; }
    string? DeletedByName { get; set; }
}

public interface IUnDeleteable
{
    bool CanBeDeleted { get; set; }
}
