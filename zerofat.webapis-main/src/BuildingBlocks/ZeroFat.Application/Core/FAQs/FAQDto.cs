using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Application.Core.FAQCategories;

namespace ZeroFat.Application.Core.FAQs;

public class FaqSimplifyDto : IDto
{
    public DefaultIdType Id { get; set; }
    public string? Question { get; set; }
    public string? Answer { get; set; }
    public bool? IsActive { get; set; }
    public FaqCategorySimplifyDto? FaqCategory { get; set; }
}

public class FaqDetailsDto : FaqSimplifyDto
{
    public List<string>? Tags { get; set; }
    public DefaultIdType FaqCategoryId { get; set; }
}
