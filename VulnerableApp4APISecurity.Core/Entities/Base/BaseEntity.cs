using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using VulnerableApp4APISecurity.Core.Interfaces.Entities.Base;

namespace VulnerableApp4APISecurity.Core.Entities.Base;

public class BaseEntity : IBaseEntity
{
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonId]
    public string? Id { get; set; }

    [BsonRepresentation(BsonType.DateTime)]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
}