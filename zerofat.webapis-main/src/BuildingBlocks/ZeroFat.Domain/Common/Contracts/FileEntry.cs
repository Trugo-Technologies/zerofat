using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ZeroFat.Domain.Enums;

namespace ZeroFat.Domain.Common.Contracts;

public interface IFileEntry
{
    DefaultIdType Id { get; set; }
    string? FileName { get; set; }
    string? Description { get; set; }
    string? Module { get; set; }
    long Size { get; set; }
    [BsonRepresentation(BsonType.String)]
    FileType FileType { get; set; }
    string? FileLocation { get; set; }
    string? FileExtension { get; set; }
    DateTime UploadedTime { get; set; }
    bool Encrypted { get; set; }
    List<FileRelatedEntity> RelatedEntities { get; set; }
}


public class FileEntry : IFileEntry
{
    [BsonId]
    public DefaultIdType Id { get; set; }

    public string? FileName { get; set; }
    public string? Description { get; set; }
    public string? Module { get; set; }
    public long Size { get; set; }

    [BsonRepresentation(BsonType.String)]
    public FileType FileType { get; set; }
    public string? FileLocation { get; set; }
    public string? FileExtension { get; set; }
    public DateTime UploadedTime { get; set; }
    public bool Encrypted { get; set; }

    [BsonElement("RelatedEntities")]
    public List<FileRelatedEntity> RelatedEntities { get; set; } = new();
}

public class FileRelatedEntity
{
    [BsonElement("EntityName")]
    public string? EntityName { get; set; }
    [BsonElement("EntityId")]
    public string? EntityId { get; set; }
    [BsonElement("IsUsed")]
    public bool IsUsed { get; set; }
}


