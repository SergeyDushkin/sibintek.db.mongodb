using System;
using System.Linq.Expressions;
using MongoDB.Driver;

namespace sibintek.db.mongodb
{
    public static partial class ODataExtension
    {
        private static FilterDefinition<T> CreatePredicate<T>(string _, string field, object value)
        {
            FilterDefinition<T> predicate = null;

            switch(_.ToLower())
            {
                case "eq":
                    predicate = CreateEqPredicate<T>(field, value);
                    break;
                case "ne":
                    predicate = CreateNePredicate<T>(field, value);
                    break;
                case "gt":
                    predicate = CreateGtPredicate<T>(field, value);
                    break;
                case "ge":
                    predicate = CreateGePredicate<T>(field, value);
                    break;
                case "lt":
                    predicate = CreateLtPredicate<T>(field, value);
                    break;
                case "le":
                    predicate = CreateLePredicate<T>(field, value);
                    break;
                default:
                    break;
            }

            return predicate;
        }
    }
}
