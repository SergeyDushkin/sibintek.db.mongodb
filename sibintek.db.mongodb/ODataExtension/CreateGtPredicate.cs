using MongoDB.Driver;

namespace sibintek.db.mongodb
{
    public static partial class ODataExtension
    {
        public static FilterDefinition<T> CreateGtPredicate<T>(string field, object value)
        {
            return Builders<T>.Filter.Gt(field, value);
        }
    }
}
