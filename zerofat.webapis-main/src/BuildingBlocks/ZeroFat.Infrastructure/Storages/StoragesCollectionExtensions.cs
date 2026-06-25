using ZeroFat.Application.Common.Interfaces;
using ZeroFat.Infrastructure.Storages.Amazon;
using ZeroFat.Infrastructure.Storages.Azure;
using ZeroFat.Infrastructure.Storages.Local;

namespace ZeroFat.Infrastructure.Storages;
public static class StoragesCollectionExtensions
{
    public static IServiceCollection AddLocalStorageManager(this IServiceCollection services, LocalOptions options)
    {
        services.AddSingleton<IFileStorageManager>(new LocalFileStorageManager(options));

        return services;
    }

    public static IServiceCollection AddAzureBlobStorageManager(this IServiceCollection services, AzureBlobOption options)
    {
        services.AddSingleton<IFileStorageManager>(new AzureBlobStorageManager(options));

        return services;
    }

    public static IServiceCollection AddAmazonS3StorageManager(this IServiceCollection services, AmazonOptions options)
    {
        services.AddSingleton<IFileStorageManager>(new AmazonS3StorageManager(options));

        return services;
    }


    public static IServiceCollection AddStorageManager(this IServiceCollection services, StorageOptions options)
    {
        if (options.UsedAzure())
        {
            services.AddAzureBlobStorageManager(options.Azure);
        }
        else if (options.UsedAmazon())
        {
            services.AddAmazonS3StorageManager(options.Amazon);
        }
        else 
        {
            services.AddLocalStorageManager(options.Local);
        }
        

        return services;
    }
}
