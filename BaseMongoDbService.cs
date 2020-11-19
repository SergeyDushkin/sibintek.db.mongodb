using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace sibintek.db.mongodb
{   
    public abstract class BaseMongoDbService<T> where T : IEntity
    {
        readonly public IMongoCollection<T> collection;
        readonly public IMongoCollection<BsonDocument> unspecifiedCollection;

        public BaseMongoDbService(IDatabaseSettings settings, string collectionName = null)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            collectionName = string.IsNullOrEmpty(collectionName) 
                ? settings.CollectionName 
                : collectionName;

            collection = database.GetCollection<T>(collectionName);
            unspecifiedCollection = database.GetCollection<BsonDocument>(collectionName);
        }


        // Полнотекстовый поиск не работает по части стоки
        private IEnumerable<T> FullTextSearch(Expression<Func<T, bool>> filter, string searchString)
        {
            var indexModel = new CreateIndexModel<T>(Builders<T>.IndexKeys.Text("$**"));
            collection.Indexes.CreateOne(indexModel);

            var builder = Builders<T>.Filter;

            var textFilter = builder.Where(filter) & builder.Text(searchString);

            var searchResult = collection.Find(textFilter).ToList();

            return searchResult;
        }

        private string[] GetSearchableFiels()
        {
            return typeof(T).GetProperties()
                .Where(r => r.PropertyType.FullName == "System.String")
                .Select(r => r.Name)
                .ToArray();
        }

        public List<T> Get() =>
            collection.Find(record => true).ToList();

        public T Get(string id) =>
            collection.Find<T>(record => record.Id == id).FirstOrDefault();

        public IQueryable<T> Get(Expression<Func<T, bool>> filter) =>
            collection.AsQueryable().Where(filter);

        public T GetBy(Expression<Func<T, bool>> filter) =>
            collection.Find<T>(filter).FirstOrDefault();
        public async Task<T> GetByAsync(Expression<Func<T, bool>> filter) =>
            await (await collection.FindAsync<T>(filter)).FirstOrDefaultAsync();
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter) =>
            await (await collection.FindAsync<T>(filter)).ToListAsync();

        public T Create(T create)
        {
            create.Id = null;
            //create.DateCreate = DateTime.Now;
            //create.DateUpdate = DateTime.Now;

            collection.InsertOne(create);
            return create;
        }

        public async Task<T> CreateAsync(T create)
        {
            create.Id = null;
            //create.DateCreate = DateTime.Now;
            //create.DateUpdate = DateTime.Now;

            await collection.InsertOneAsync(create);
            return await Task.FromResult(create);
        }

        public async Task CreateAsync(BsonDocument create)
        {
            await unspecifiedCollection.InsertOneAsync(create);
        }
        
        public T Update(string id, T @record)
        {
            //@record.DateUpdate = DateTime.Now;
            collection.ReplaceOne(r => r.Id == id, @record);
            
            return @record;
        }
        
        public async Task<T> UpdateAsync(string id, T @record)
        {
            //@record.DateUpdate = DateTime.Now;
            await collection.ReplaceOneAsync(r => r.Id == id, @record);
            
            return await Task.FromResult(@record);
        }

        public async Task<T> UpdateAsync(Expression<Func<T, bool>>filter, T @record)
        {
            //@record.DateUpdate = DateTime.Now;
            await collection.ReplaceOneAsync(filter, @record);
            
            return await Task.FromResult(@record);
        }

        public void Remove(string id) => 
            collection.DeleteOne(r => r.Id == id);

        public bool Exists(Expression<Func<T, bool>>filter) =>
            collection.CountDocuments(filter) > 0;
        public async Task<bool> ExistsAsync(Expression<Func<T, bool>>filter) =>
            (await collection.CountDocumentsAsync(filter)) > 0;
    }
}


        /*
        public IEnumerable<T> Search(Expression<Func<T, bool>> filter, string searchString)
        {
            var pattern = ".*" + searchString + ".*";
            var builder = Builders<T>.Filter;

            FilterDefinition<T> textFilter = null;

            foreach(var field in GetSearchableFiels())
            {
                if (textFilter == null)
                {
                    textFilter = builder.Regex(field, BsonRegularExpression.Create(new Regex(pattern, RegexOptions.IgnoreCase)));
                }

                textFilter = textFilter | builder.Regex(field, BsonRegularExpression.Create(new Regex(pattern, RegexOptions.IgnoreCase)));
            }

            var resultFilter = textFilter == null 
                ? builder.Where(filter) 
                : builder.Where(filter) & textFilter;

            var searchResult = collection.Find(resultFilter)
                .Skip(0)
                .Limit(100)
                .SortByDescending(r => r.DateUpdate)
                .ToList();

            return searchResult;
        }
        */
