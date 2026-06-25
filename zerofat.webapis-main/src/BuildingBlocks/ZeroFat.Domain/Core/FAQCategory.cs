using ZeroFat.Domain.Core.Common;

namespace ZeroFat.Domain.Core;

public class FaqCategory : ActivationEntity, ICoreAggregateRoot
{
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public virtual ICollection<Faq> FAQs { get; set; } = new HashSet<Faq>();
}
