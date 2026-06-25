using Microsoft.AspNetCore.Http;
using ZeroFat.Domain.Common.Contracts;
using ZeroFat.Domain.Enums;

namespace ZeroFat.Application.Common.Interfaces;

public interface IFileStorageManager
{
    Task CreateAsync(IFileEntry fileEntry, Stream stream, CancellationToken cancellationToken = default);

    Task<byte[]> ReadAsync(IFileEntry fileEntry, CancellationToken cancellationToken = default);
    Task<byte[]> ReadAsync(string key, CancellationToken cancellationToken = default);
    Task DeleteAsync(IFileEntry fileEntry, CancellationToken cancellationToken = default);

    Task ArchiveAsync(IFileEntry fileEntry, CancellationToken cancellationToken = default);

    Task UnArchiveAsync(IFileEntry fileEntry, CancellationToken cancellationToken = default);

    Task<string> UploadAsync<T>(IFormFile? file, FileType supportedFileType, string module, CancellationToken cancellationToken = default);
    Task<string> UploadThumbnailAsync<T>(IFormFile? file, FileType supportedFileType, string module, CancellationToken cancellationToken = default);

    Task<string> UploadNormalAsync<T>(IFormFile? file, FileType supportedFileType, string module, CancellationToken cancellationToken = default);
}

