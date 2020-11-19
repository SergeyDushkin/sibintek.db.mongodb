using MongoDB.Driver;

namespace sibintek.db.mongodb
{
    public static partial class ODataExtension
    {
        public static FilterDefinition<T> CreateNePredicate<T>(string field, object value)
        {
            return Builders<T>.Filter.Ne(field, value);
        }
    }
}
