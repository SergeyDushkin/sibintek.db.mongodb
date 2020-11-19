using System;
using System.Linq;
using MongoDB.Driver;
using sibintek.sibmobile.core;

namespace sibintek.db.mongodb
{
    public static partial class ODataExtension
    {
        private static SortDefinition<T> CreateODataSort<T>(IODataQuery query, bool check = false)
        {
            var orderby = query.Order;

            SortDefinitionBuilder<T> sortBuilder = Builders<T>.Sort;    
            SortDefinition<T> sortDefinition = null;
            SortDefinition<T> defaultSortDefinition = sortBuilder.Descending("CreateDate");

            if (String.IsNullOrEmpty(orderby))
            {
                return defaultSortDefinition;
            }

            var sections = orderby.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach(var section in sections)
            {
                var order = section.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                var field = order.FirstOrDefault();
                var direction = "asc";
                
                if (check && !typeof(T).HasField(field))
                {
                    continue;
                }

                if (order.LastOrDefault().ToLower() == "desc")
                {
                    direction = "desc";
                }
                
                if (sortDefinition == null)
                {
                    sortDefinition = direction == "asc" 
                        ? sortBuilder.Ascending(field) 
                        : sortBuilder.Descending(field);
                }

                sortDefinition = direction == "asc" 
                    ? sortDefinition.Ascending(field) 
                    : sortDefinition.Descending(field);
            }

            return sortDefinition ?? defaultSortDefinition;
        }

        private static bool HasField(this Type type, string field)
        {
            return type.GetProperties().Any(r => r.Name.ToLower() == field.ToLower());
        }
    }
}
