using System;
using System.Linq;
using MongoDB.Driver;
using sibintek.sibmobile.core;

namespace sibintek.db.mongodb
{
    public static partial class ODataExtension
    {
        private static FilterDefinition<T> CreateODataFilter<T>(IODataQuery query, bool check = false)
        {
            var filter = query.Filter;

            if (String.IsNullOrEmpty(filter))
            {
                return FilterDefinition<T>.Empty;
            }

            var sections = filter.Split(new string[] { " and ", " or " }, StringSplitOptions.RemoveEmptyEntries);
            var firstSection = sections.FirstOrDefault();

            var fieldIdx = firstSection.IndexOf(" ");
            var operatorIdx = firstSection.IndexOf(" ", fieldIdx + 1);

            var field = firstSection.Substring(0, fieldIdx);
            var operatorName = firstSection.Substring(fieldIdx + 1, operatorIdx - fieldIdx - 1);
            var stringValue = firstSection.Substring(operatorIdx + 1);

            object value = stringValue;

            if (check)
            {
                var propery = typeof(T)
                    .GetProperties()
                    .Where(r => r.Name.ToLower() == field.ToLower())
                    .SingleOrDefault();

                if (propery == null)
                {
                    return FilterDefinition<T>.Empty;
                }
                
                value = Cast(stringValue, propery.PropertyType);
            }

            if (value == null)
            {
                return FilterDefinition<T>.Empty;
            }
            
            return CreatePredicate<T>(operatorName, field, value);
        }
    }
}
