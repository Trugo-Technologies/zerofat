using ZeroFat.Domain.Core.Common;

namespace ZeroFat.Domain.Core;

public class Banner : ActivationEntity, ICoreAggregateRoot
{
    public string? TitleEn { get; set; }
    public string? TitleAr { get; set; }
    public int Index { get; set; }
    public string? DescriptionEn { get; set; }
    public string? DescriptionAr { get; set; }
    public string? ImageUrl { get; set; }
    public string? ImageThumbnailUrl { get; set; }
    public string? ImageOptimzeUrl { get; set; }
}
