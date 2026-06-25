using ZeroFat.Domain.Core.Common;

namespace ZeroFat.Domain.Core;

public class PhysicalActivityLevel : ActivationEntity, ICoreAggregateRoot
{
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public string? ExampleEn { get; set; }
    public string? ExampleAr { get; set; }
    public double ActivityValue { get; set; }

    public string? ImageUrl { get; set; }
    public string? ImageThumbnailUrl { get; set; }
    public string? ImageOptimzeUrl { get; set; }
}
