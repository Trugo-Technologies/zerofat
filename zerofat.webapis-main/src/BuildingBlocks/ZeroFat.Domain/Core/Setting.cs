using ZeroFat.Domain.Core.Common;
using ZeroFat.Domain.Enums;

namespace ZeroFat.Domain.Core;

public class Setting : Entity, ICoreAggregateRoot
{
    public string PropertyName { get; set; } = default!;
    public PropertyType PropertyType { get; set; }
    public ApplicationModule ApplicationModule { get; set; }
    public string? Value { get; set; }
    public bool CanBeDeleted { get; set; }
    public bool CanBeEdited { get; set; } = true;
}
