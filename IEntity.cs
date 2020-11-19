using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace sibintek.db.mongodb
{   
    public interface IEntity
    {
        string Id { get; set; }
    }

    public interface IAudit
    {
        string AuthorCreate { get; set; }
        DateTimeOffset DateCreate { get; set; }

        string AuthorUpdate { get; set; }
        DateTimeOffset DateUpdate { get; set; }
    }

    public abstract class Entity : IEntity, IAudit
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string AuthorCreate { get; set; }
        public DateTimeOffset DateCreate { get; set; }
        public string AuthorUpdate { get; set; }
        public DateTimeOffset DateUpdate { get; set; }
    }
}
