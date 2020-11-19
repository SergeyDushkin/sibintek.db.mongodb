using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace sibintek.db.mongodb
{
    public static class BsonDocumentExtension
    {
        public static T Get<T>(this BsonDocument document, Func<T, Object>  field)
        {
            return BsonSerializer.Deserialize<T>(document.GetValue(nameof(field)).ToJson()); 
        }

        public static T Get<T>(this BsonDocument document, string field)
        {
            return BsonSerializer.Deserialize<T>(document.GetValue(field).ToJson()); 
        }

        public static object Get(this BsonDocument document, string field, Type type)
        {
            return BsonSerializer.Deserialize(document.GetValue(field).ToJson(), type); 
        }
    }   
}
