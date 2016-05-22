using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using Repository.JsonConverters;

namespace Repository
{
    public class MongoRepository<T> : IRepository<T>
        where T : GuidEntity
    {
        private readonly string _collectionName;

        protected MongoRepository(IMongoClient client, string dbName)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            if (string.IsNullOrEmpty(dbName))
                throw new ArgumentNullException(nameof(dbName));
            Client = client;
            DataBase = Client.GetDatabase(dbName);

            var attr = Attribute.GetCustomAttribute(typeof(T), typeof(MongoCollectionName)) as MongoCollectionName;
            if (attr != null && !string.IsNullOrEmpty(attr.Name))
                _collectionName = attr.Name;
            else
                _collectionName = typeof(T).Name;
        }

        protected MongoRepository(string connectionName) :
            this(new MongoClient(new MongoUrl(connectionName)), new MongoUrl(connectionName).DatabaseName)
        {
        }

        private IMongoClient Client { get; }

        private IMongoDatabase DataBase { get; }

        private IMongoCollection<T> Collection => DataBase.GetCollection<T>(_collectionName);

        #region IQuerable
        public Type ElementType => Collection.AsQueryable().ElementType;

        public Expression Expression => Collection.AsQueryable().Expression;

        public IQueryProvider Provider => Collection.AsQueryable().Provider;

        public IEnumerator<T> GetEnumerator()
        {
            return Collection.AsQueryable().GetEnumerator();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return Collection.AsQueryable().GetEnumerator();
        }

        #endregion

        protected virtual void PreAddItem(T entity)
        {

        }

        public void Save(T entity, bool isUpsert = false)
        {
            if (isUpsert)
                PreAddItem(entity);
            Collection.ReplaceOne(x => x.Id == entity.Id, entity, new UpdateOptions { IsUpsert = isUpsert });
        }

        public Task SaveAsync(T entity, bool isUpsert = false, CancellationToken token = default(CancellationToken))
        {
            if (isUpsert)
                PreAddItem(entity);
            return Collection.ReplaceOneAsync(x => x.Id == entity.Id, entity, new UpdateOptions { IsUpsert = isUpsert }, token);
        }

        public void SavePartial(T entity, params Expression<Func<T, object>>[] updates)
        {
            var updateDefinition = Builders<BsonDocument>.Update.Combine(ConvertUpdateDefinition(entity, updates));
            DataBase.GetCollection<BsonDocument>(_collectionName).UpdateOne(x => x["_id"] == entity.Id, updateDefinition);
        }

        public Task SavePartialAsync(T entity, CancellationToken token = default(CancellationToken), params Expression<Func<T, object>>[] updates)
        {
            var updateDefinition = Builders<BsonDocument>.Update.Combine(ConvertUpdateDefinition(entity, updates));
            return DataBase.GetCollection<BsonDocument>(_collectionName).UpdateOneAsync(x => x["_id"] == entity.Id, updateDefinition, null, token);
        }

        private IEnumerable<UpdateDefinition<BsonDocument>> ConvertUpdateDefinition(T entity, params Expression<Func<T, object>>[] updates)
        {
            var map = (BsonClassMap<T>)BsonClassMap.LookupClassMap(typeof(T));
            return (from update in updates
                    let fieldMap = map.GetMemberMap(update)
                    let exp = update.Compile()
                    let value = exp(entity)
                    let sz = JsonConvert.SerializeObject(value, new JsonSerializerSettings
                    {
                        ContractResolver = new BsonDataContractResolver(),
                        Converters = new List<JsonConverter> { new LongJsonConverter(), new DateTimeJsonConverter() }
                    })
                    select $"{{$set : {{{fieldMap.ElementName} : {sz}}}}}").Select(updateStr => (UpdateDefinition<BsonDocument>)updateStr);
        }

        public void Add(T entity)
        {
            PreAddItem(entity);
            Collection.InsertOne(entity);
        }

        public Task AddAsync(T entity, CancellationToken token = default(CancellationToken))
        {
            PreAddItem(entity);
            return Collection.InsertOneAsync(entity, new InsertOneOptions { BypassDocumentValidation = false }, token);
        }

        public void AddMany(IEnumerable<T> entities)
        {
            Collection.InsertMany(entities);
        }

        public Task AddManyAsync(IEnumerable<T> entities, CancellationToken token = default(CancellationToken))
        {
            return Collection.InsertManyAsync(entities, null, token);
        }

        public void Delete(T entity)
        {
            Collection.DeleteOne(x => x.Id == entity.Id);
        }

        public Task DeleteAsync(T entity, CancellationToken token = default(CancellationToken))
        {
            return Collection.DeleteOneAsync(x => x.Id == entity.Id, token);
        }

        public void DeleteMany(IEnumerable<T> entities)
        {
            var listIds = entities.Select(x => x.Id).ToList();
            Collection.DeleteMany(x => listIds.Contains(x.Id));
        }

        public Task DeleteManyAsync(IEnumerable<T> entities, CancellationToken token = default(CancellationToken))
        {
            var listIds = entities.Select(x => x.Id).ToList();
            return Collection.DeleteManyAsync(x => listIds.Contains(x.Id), token);
        }
    }
}
