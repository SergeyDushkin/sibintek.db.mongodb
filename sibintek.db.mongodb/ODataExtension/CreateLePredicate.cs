using MongoDB.Driver;

namespace sibintek.db.mongodb
{
    public static partial class ODataExtension
    {
        public static FilterDefinition<T> CreateLePredicate<T>(string field, object value)
        {
            return Builders<T>.Filter.Lte(field, value);
        }
    }
}