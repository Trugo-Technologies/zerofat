using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using ZeroFat.Application.Common.Extensions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Common.Contracts;
using ZeroFat.Domain.Enums;

namespace ZeroFat.Infrastructure.Storages.Local;

public class LocalFileStorageManager : IFileStorageManager
{
    private readonly LocalOptions _option;

    public LocalFileStorageManager(LocalOptions option)
    {
        _option = option;
    }

    public async Task CreateAsync(IFileEntry fileEntry, Stream stream, CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(_option.Path, fileEntry.FileLocation);

        var folder = Path.GetDirectoryName(filePath);

        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        using (var fileStream = File.Create(filePath))
        {
            await stream.CopyToAsync(fileStream, cancellationToken);
        }
    }

    public async Task DeleteAsync(IFileEntry fileEntry, CancellationToken cancellationToken = default)
    {
        await Task.Run(() =>
        {
            var path = Path.Combine(_option.Path, fileEntry.FileLocation);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }, cancellationToken);
    }

    public Task<byte[]> ReadAsync(IFileEntry fileEntry, CancellationToken cancellationToken = default)
    {
        return File.ReadAllBytesAsync(Path.Combine(_option.Path, fileEntry.FileLocation), cancellationToken);
    }

    public Task<byte[]> ReadAsync(string key, CancellationToken cancellationToken = default)
    {
        return File.ReadAllBytesAsync(Path.Combine(_option.Path, key), cancellationToken);
    }

    public Task ArchiveAsync(IFileEntry fileEntry, CancellationToken cancellationToken = default)
    {
        // TODO: move to archive storage
        return Task.CompletedTask;
    }

    public Task UnArchiveAsync(IFileEntry fileEntry, CancellationToken cancellationToken = default)
    {
        // TODO: move to active storage
        return Task.CompletedTask;
    }

    public async Task<string> UploadAsync<T>(IFormFile? file, FileType supportedFileType, string module, CancellationToken cancellationToken = default)
    {
        if (file == null)
            return string.Empty;

        string fileExtension = Path.GetExtension(file.FileName);
        if (!supportedFileType.GetDescriptionList().Contains(fileExtension) && supportedFileType != FileType.Other)
            throw new InvalidOperationException("File format not supported.");

        string folder = typeof(T).Name;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            folder = folder.Replace(@"\", "/");
        }

        string folderName = supportedFileType switch
        {
            FileType.Image => Path.Combine("Files", module, "Images", folder),
            _ => Path.Combine("Files", module, "Others", folder),
        };

        string pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

        if (!Directory.Exists(pathToSave))
            Directory.CreateDirectory(pathToSave);

        string fileName = Guid.NewGuid().ToString() + fileExtension;

        string fullPath = Path.Combine(pathToSave, fileName);
        string dbPath = Path.Combine(folderName, fileName);

        using var fileStream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(fileStream);
        return dbPath.Replace("\\", "/");
    }

    public async Task<string> UploadNormalAsync<T>(IFormFile? file, FileType supportedFileType, string module, CancellationToken cancellationToken = default)
    {
        if (file == null)
            return string.Empty;

        string fileExtension = Path.GetExtension(file.FileName);
        if (!supportedFileType.GetDescriptionList().Contains(fileExtension) && supportedFileType != FileType.Other)
            throw new InvalidOperationException("File format not supported.");

        string folder = typeof(T).Name;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            folder = folder.Replace(@"\", "/");
        }

        string folderName = supportedFileType switch
        {
            FileType.Image => Path.Combine("Files", module, "Images", folder, "normals"),
            _ => Path.Combine("Files", module, "Others", folder),
        };

        string pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

        if (!Directory.Exists(pathToSave))
            Directory.CreateDirectory(pathToSave);

        string fileName = Guid.NewGuid().ToString() + fileExtension;

        string fullPath = Path.Combine(pathToSave, fileName);
        string dbPath = Path.Combine(folderName, fileName);

        using var fileStream = new FileStream(fullPath, FileMode.Create);
        var encoder = new JpegEncoder() { Quality = 90 };
        Image image = await Image.LoadAsync(file.OpenReadStream(), cancellationToken);
        while (image.Height >= 1600 || image.Width >= 1600)
        {
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(1200, 1200),
                Mode = ResizeMode.Max
            }));
        }

        await image.SaveAsync(fileStream, encoder, cancellationToken);

        return dbPath.Replace("\\", "/");

    }

    public async Task<string> UploadThumbnailAsync<T>(IFormFile? file, FileType supportedFileType, string module, CancellationToken cancellationToken = default)
    {
        if (file == null)
            return string.Empty;

        string fileExtension = Path.GetExtension(file.FileName);
        if (!supportedFileType.GetDescriptionList().Contains(fileExtension) && supportedFileType != FileType.Other)
            throw new InvalidOperationException("File format not supported.");

        string folder = typeof(T).Name;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            folder = folder.Replace(@"\", "/");
        }

        string folderName = supportedFileType switch
        {
            FileType.Image => Path.Combine("Files", module, "Images", folder, "thumbnails"),
            _ => Path.Combine("Files", module, "Others", folder),
        };

        string pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

        if (!Directory.Exists(pathToSave))
            Directory.CreateDirectory(pathToSave);

        string fileName = Guid.NewGuid().ToString() + fileExtension;

        string fullPath = Path.Combine(pathToSave, fileName);
        string dbPath = Path.Combine(folderName, fileName);

        using var fileStream = new FileStream(fullPath, FileMode.Create);
        var encoder = new JpegEncoder() { Quality = 60 };
        Image image = await Image.LoadAsync(file.OpenReadStream(), cancellationToken);
        while (image.Height >= 1000 || image.Width >= 1000)
        {
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(600, 600),
                Mode = ResizeMode.Max
            }));
        }

        await image.SaveAsync(fileStream, encoder, cancellationToken);

        return dbPath.Replace("\\", "/");

    }
}
