namespace ZeroFat.Domain.Common.Contracts;

public interface IImageEntity
{
    string? GetOriginalImageUrl();
    string? GetThumbnailUrl();
    string? GetOptimizedUrl();

    void SetThumbnailUrl(string url);
    void SetOptimizedUrl(string url);
}
