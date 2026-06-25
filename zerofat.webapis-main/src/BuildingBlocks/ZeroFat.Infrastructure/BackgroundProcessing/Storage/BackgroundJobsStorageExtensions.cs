using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using MongoDB.Driver;

namespace ZeroFat.Infrastructure.BackgroundProcessing.Storage;

internal static class BackgroundJobsStorageExtensions
{
    private const string CollectionPrefix = "hangfire";

    private static MongoStorageOptions MongoStorageOptions => new()
    {
        MigrationOptions = new MongoMigrationOptions
        {
            MigrationStrategy = new MigrateMongoMigrationStrategy(),
            BackupStrategy = new CollectionMongoBackupStrategy()
        },
        Prefix = CollectionPrefix,
        CheckConnection = true
    };

    internal static IGlobalConfiguration UseMongoAsStorage(this IGlobalConfiguration hangfireConfiguration, string connectionString)
    {
        var mongoUrlBuilder = new MongoUrlBuilder(connectionString);
        var mongoClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());

        hangfireConfiguration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseMongoStorage(mongoClient, mongoUrlBuilder.DatabaseName, MongoStorageOptions);

        return hangfireConfiguration;
    }
}
