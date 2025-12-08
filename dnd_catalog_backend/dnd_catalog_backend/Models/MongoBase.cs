// Models/MongoBase.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace dnd_catalog_backend.Models
{
    public abstract class MongoBase
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
    }
}