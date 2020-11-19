using System;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using sibintek.sibmobile.core;

namespace sibintek.db.mongodb
{
    public static partial class ODataExtension
    {
        public static async Task<PagedCollection<T>> ApplyOData<T>(this IMongoCollection<T> collection, IODataQuery query)
        {
            var page = 1;
            var skip = query.Skip.Normalize(0);
            var limit = query.Limit.Normalize(10, 1000);

            var countFacet = AggregateFacet.Create("count",
                PipelineDefinition<T, AggregateCountResult>.Create(new[]
                {
                    PipelineStageDefinitionBuilder.Count<T>()
                }));

            var sort = CreateODataSort<T>(query, false);
            var filter = CreateODataFilter<T>(query, false);

            var dataFacet = AggregateFacet.Create("data",
                PipelineDefinition<T, T>.Create(new []
                {
                    PipelineStageDefinitionBuilder.Sort(sort),
                    PipelineStageDefinitionBuilder.Skip<T>(skip),
                    PipelineStageDefinitionBuilder.Limit<T>(limit),
                }));

            var aggregation = await collection.Aggregate()
                .Match(filter)
                .Facet(countFacet, dataFacet)
                .ToListAsync();

            var count = aggregation.First()
                .Facets.First(x => x.Name == "count")
                .Output<AggregateCountResult>()
                .First()
                .Count;

            int totalPages = (int)(count / limit);

            var data = aggregation.First()
                .Facets.First(x => x.Name == "data")
                .Output<T>();

            return PagedCollection<T>.Create(data, page, limit, totalPages, count);
        }
    }
}
