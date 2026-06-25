using ZeroFat.Domain.Core.Common;

namespace ZeroFat.Domain.Core;

public class Faq : ActivationEntity, ICoreAggregateRoot
{
    public string Question { get; set; } = default!;
    public string Answer { get; set; } = default!;
    public DefaultIdType FaqCategoryId { get; set; }
    public List<string>? Tags { get; set; } = [];
    public virtual FaqCategory? FaqCategory { get; set; }
}
