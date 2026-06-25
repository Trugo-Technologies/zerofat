using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ZeroFat.Application.Common.CQRS;
using ZeroFat.Application.Common.Extensions;
using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Domain.Common.Contracts;
using ZeroFat.Domain.Enums;
using ZeroFat.GymUp.Application.Contracts;
using ZeroFat.GymUp.Domain;
using ZeroFat.GymUp.Infrastructure.Persistence.Context;
using ZeroFat.Infrastructure.BackgroundProcessing.Contracts;

namespace ZeroFat.GymUp.Infrastructure.Services;
internal sealed class WorkoutModule : IWorkoutModule
{
    private readonly IMediator _mediator;

    public WorkoutModule(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task ExecuteCommandAsync(ICommand command)
    {
        await _mediator.Send(command);
    }

    public async Task<TResult> ExecuteCommandAsync<TResult>(ICommand<TResult> command)
        where TResult : notnull
    {
        return await _mediator.Send(command);
    }

    public async Task<TResult> ExecuteQueryAsync<TResult>(IQuery<TResult> query)
         where TResult : notnull
    {
        return await _mediator.Send(query);
    }
}

public interface IImageBackfillJob : IScopedService
{
    Task RunAsync();
}

public class WorkoutImageBackfillJob : IRecurringBackgroundJobScheduler
{
    private readonly IImageBackfillJob _calculator;

    public WorkoutImageBackfillJob(IImageBackfillJob calculator)
    {
        _calculator = calculator;
    }

    public void Schedule(IRecurringJobManager recurringJobManager)
    {
        recurringJobManager.AddOrUpdate(
            "workout-image-backfill-job",
            () => _calculator.RunAsync(),
            Cron.Daily(3)); // 3 AM UTC
    }
}


public class ImageBackfillJob : IImageBackfillJob
{
    private readonly GymUpContext _db;
    private readonly IImageProcessor _imageProcessor; // the one we built earlier
    private readonly CancellationToken _cancellationToken;

    public ImageBackfillJob(GymUpContext db, IImageProcessor imageProcessor)
    {
        _db = db;
        _imageProcessor = imageProcessor;
    }

    public async Task RunAsync()
    {
        // await ProcessEntityListAsync<Equipment>(_db.Equipments, FileType.Image, ModuleConstant.ModuleName);

        //await ProcessEntityListAsync<PlanGoal>(_db.PlanGoals, FileType.Image, ModuleConstant.ModuleName);
        //await ProcessEntityListAsync<Plan>(_db.Plans, FileType.Image, ModuleConstant.ModuleName);
        //await ProcessEntityListAsync<Exercise>(_db.Exercises, FileType.Image, ModuleConstant.ModuleName);

        await ProcessEntityListAsync<Workout>(_db.Workouts, FileType.Image, ModuleConstant.ModuleName);
        await ProcessEntityListAsync<Trainer>(_db.Trainers, FileType.Image, ModuleConstant.ModuleName);

        await _db.SaveChangesAsync(_cancellationToken);
    }

    private async Task ProcessEntityListAsync<TEntity>(
        IQueryable<TEntity> query,
        FileType fileType,
        string module)
        where TEntity : class, IImageEntity
    {
        var list = await query
        .ToListAsync(_cancellationToken);

        foreach (var entity in list.Where(x => x.GetOriginalImageUrl() != null && (x.GetThumbnailUrl() == null || x.GetOptimizedUrl() == null)))
        {
            try
            {
                await _imageProcessor.ProcessAsync<TEntity>(entity, fileType, module);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing {typeof(TEntity).Name} with ID : {ex.Message}");
            }
        }

        foreach (var entity in list)
        {
            try
            {
                await _imageProcessor.ProcessAsync<TEntity>(entity, fileType, module);
            }
            catch (Exception ex)
            {
                // Log and continue
                Console.WriteLine($"Failed to process {typeof(TEntity).Name} ID: , Error: {ex.Message}");
            }
        }
    }
}

public interface IImageProcessor : IScopedService
{
    Task ProcessAsync<T>(IImageEntity entity, FileType fileType, string module) where T : IImageEntity;
}

public class ImageProcessor : IImageProcessor
{
    private readonly IFileStorageManager _storageService;

    public ImageProcessor(IFileStorageManager storageService)
    {
        _storageService = storageService;
    }

    public async Task ProcessAsync<T>(
        IImageEntity entity,
        FileType fileType,
        string module) where T : IImageEntity
    {
        var imageUrl = entity.GetOriginalImageUrl();
        if (string.IsNullOrWhiteSpace(imageUrl))
            return;

        var imageBytes = await _storageService.ReadAsync(imageUrl);

        using var stream = new MemoryStream(imageBytes);
        var formFile = new FormFile(stream, 0, stream.Length, "image", Path.GetFileName(imageUrl))
        {
            Headers = new HeaderDictionary(),
            ContentType = GetContentType(imageUrl)
        };

        if (entity.GetThumbnailUrl().IsEmpty())
        {
            var thumbnailUrl = await _storageService.UploadThumbnailAsync<T>(formFile, fileType, module);
            entity.SetThumbnailUrl(thumbnailUrl);
        }

        if (entity.GetOptimizedUrl().IsEmpty())
        {
            var optimizedUrl = await _storageService.UploadNormalAsync<T>(formFile, fileType, module);
            entity.SetOptimizedUrl(optimizedUrl);
        }

       
    }

    private string ExtractS3KeyFromUrl(string url)
    {
        // Example: https://your-bucket.s3.amazonaws.com/folder/image.jpg
        // This will return: folder/image.jpg
        var uri = new Uri(url);
        return uri.AbsolutePath.TrimStart('/');
    }

    private string GetContentType(string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            _ => "application/octet-stream"
        };
    }
}
