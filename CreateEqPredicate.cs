using MongoDB.Driver;

namespace sibintek.db.mongodb
{
    public static partial class ODataExtension
    {
        public static FilterDefinition<T> CreateEqPredicate<T>(string field, object value)
        {
            return Builders<T>.Filter.Eq(field, value);
        }
    }
}
