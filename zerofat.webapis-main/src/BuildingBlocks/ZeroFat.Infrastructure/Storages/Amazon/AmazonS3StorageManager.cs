using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using ZeroFat.Application.Common.Extensions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Common.Contracts;
using ZeroFat.Domain.Enums;

namespace ZeroFat.Infrastructure.Storages.Amazon;


public class AmazonS3StorageManager : IFileStorageManager
{
    private readonly IAmazonS3 _client;
    private readonly AmazonOptions _options;

    public AmazonS3StorageManager(AmazonOptions options)
    {
        _client = new AmazonS3Client(options.AccessKeyID, options.SecretAccessKey, RegionEndpoint.GetBySystemName(options.RegionEndpoint));
        _options = options;
    }

    private string GetKey(IFileEntry fileEntry)
    {
        return Path.Combine(fileEntry.FileLocation).Replace("\\", "/");
    }

    private string GenerateKey<T>(FileType supportedFileType, string module, string fileName, string subFolder = "")
    {
        string folder = typeof(T).Name;
        string folderName = supportedFileType switch
        {
            FileType.Image => Path.Combine("Files", module, "Images", folder, subFolder),
            _ => Path.Combine("Files", module, "Others", folder, subFolder),
        };

        return Path.Combine(folderName, fileName).Replace("\\", "/");
    }

    public async Task CreateAsync(IFileEntry fileEntry, Stream stream, CancellationToken cancellationToken = default)
    {
        var uploadRequest = new TransferUtilityUploadRequest
        {
            InputStream = stream,
            Key = GetKey(fileEntry),
            BucketName = _options.BucketName,
            CannedACL = S3CannedACL.PublicRead,
            AutoCloseStream = false
        };

        var fileTransferUtility = new TransferUtility(_client);
        await fileTransferUtility.UploadAsync(uploadRequest, cancellationToken);
    }

    public async Task DeleteAsync(IFileEntry fileEntry, CancellationToken cancellationToken = default)
    {
        await _client.DeleteObjectAsync(new DeleteObjectRequest
        {
            BucketName = _options.BucketName,
            Key = GetKey(fileEntry)
        }, cancellationToken);
    }

    public async Task<byte[]> ReadAsync(IFileEntry fileEntry, CancellationToken cancellationToken = default)
    {
        using var response = await _client.GetObjectAsync(new GetObjectRequest
        {
            BucketName = _options.BucketName,
            Key = GetKey(fileEntry)
        }, cancellationToken);

        await using var responseStream = response.ResponseStream;
        using var memoryStream = new MemoryStream();
        await responseStream.CopyToAsync(memoryStream, cancellationToken);
        return memoryStream.ToArray();
    }

    public async Task<byte[]> ReadAsync(string key, CancellationToken cancellationToken = default)
    {
        using var response = await _client.GetObjectAsync(new GetObjectRequest
        {
            BucketName = _options.BucketName,
            Key = key
        }, cancellationToken);

        await using var responseStream = response.ResponseStream;
        using var memoryStream = new MemoryStream();
        await responseStream.CopyToAsync(memoryStream, cancellationToken);
        return memoryStream.ToArray();
    }


    public async Task ArchiveAsync(IFileEntry fileEntry, CancellationToken cancellationToken = default)
    {
        await _client.CopyObjectAsync(new CopyObjectRequest
        {
            SourceBucket = _options.BucketName,
            SourceKey = GetKey(fileEntry),
            DestinationBucket = _options.BucketName,
            DestinationKey = GetKey(fileEntry),
            StorageClass = S3StorageClass.StandardInfrequentAccess
        }, cancellationToken);
    }

    public async Task UnArchiveAsync(IFileEntry fileEntry, CancellationToken cancellationToken = default)
    {
        await _client.CopyObjectAsync(new CopyObjectRequest
        {
            SourceBucket = _options.BucketName,
            SourceKey = GetKey(fileEntry),
            DestinationBucket = _options.BucketName,
            DestinationKey = GetKey(fileEntry),
            StorageClass = S3StorageClass.Standard
        }, cancellationToken);
    }

    public async Task<string> UploadAsync<T>(IFormFile? file, FileType supportedFileType, string module, CancellationToken cancellationToken = default)
    {
        if (file == null) return string.Empty;

        string fileExtension = Path.GetExtension(file.FileName);
        if (!supportedFileType.GetDescriptionList().Contains(fileExtension) && supportedFileType != FileType.Other)
            throw new InvalidOperationException("File format not supported.");

        string fileName = $"{Guid.NewGuid()}{fileExtension}";
        string key = GenerateKey<T>(supportedFileType, module, fileName);

        await using var stream = file.OpenReadStream();
        await CreateAsync(new FileEntry { FileLocation = key }, stream, cancellationToken);

        return key;
    }

    public async Task<string> UploadNormalAsync<T>(IFormFile? file, FileType supportedFileType, string module, CancellationToken cancellationToken = default)
    {
        if (file == null) return string.Empty;

        string fileExtension = Path.GetExtension(file.FileName);
        if (!supportedFileType.GetDescriptionList().Contains(fileExtension) && supportedFileType != FileType.Other)
            throw new InvalidOperationException("File format not supported.");

        string fileName = $"{Guid.NewGuid()}{fileExtension}";
        string key = GenerateKey<T>(supportedFileType, module, fileName, "normals");

        await using var outputStream = new MemoryStream();
        var encoder = new JpegEncoder { Quality = 90 };

        using (Image image = await Image.LoadAsync(file.OpenReadStream(), cancellationToken))
        {
            while (image.Height >= 1600 || image.Width >= 1600)
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(1200, 1200),
                    Mode = ResizeMode.Max
                }));
            }
            await image.SaveAsync(outputStream, encoder, cancellationToken);
        }

        outputStream.Position = 0;
        await CreateAsync(new FileEntry { FileLocation = key }, outputStream, cancellationToken);

        return key;
    }

    public async Task<string> UploadThumbnailAsync<T>(IFormFile? file, FileType supportedFileType, string module, CancellationToken cancellationToken = default)
    {
        if (file == null) return string.Empty;

        string fileExtension = Path.GetExtension(file.FileName);
        if (!supportedFileType.GetDescriptionList().Contains(fileExtension) && supportedFileType != FileType.Other)
            throw new InvalidOperationException("File format not supported.");

        string fileName = $"{Guid.NewGuid()}{fileExtension}";
        string key = GenerateKey<T>(supportedFileType, module, fileName, "thumbnails");

        await using var outputStream = new MemoryStream();
        var encoder = new JpegEncoder { Quality = 60 };

        using (Image image = await Image.LoadAsync(file.OpenReadStream(), cancellationToken))
        {
            while (image.Height >= 1000 || image.Width >= 1000)
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(600, 600),
                    Mode = ResizeMode.Max
                }));
            }
            await image.SaveAsync(outputStream, encoder, cancellationToken);
        }

        outputStream.Position = 0;
        await CreateAsync(new FileEntry { FileLocation = key }, outputStream, cancellationToken);

        return key;
    }

   
}
