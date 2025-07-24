using DatabaseContext.DataLayer;
using DatabaseContext.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace DatabaseContext.Repositories
{
    public class SpinLogicRepository : ISpinLogicRepository
    {
        private readonly IMongoCollection<BsonDocument> _busyUsersCollection;
        private readonly IMongoCollection<BsonDocument> _busyUsersCollectionHistory;

        public SpinLogicRepository(IMongoDatabase database)
        {
            _busyUsersCollection = database.GetCollection<BsonDocument>("Balance_Withdrawals");
            _busyUsersCollectionHistory = database.GetCollection<BsonDocument>("Balance_Withdrawals_History");
        }

        public async Task InsertAsync(string key, SpinLogicDataModelMongo dataModel)
        {
            var bsonDocument = CreateBsonDocument(key, dataModel);
            await _busyUsersCollection.InsertOneAsync(bsonDocument);
            await _busyUsersCollectionHistory.InsertOneAsync(bsonDocument);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("key", key);
            var count = await _busyUsersCollection.CountDocumentsAsync(filter);
            return count > 0;
        }

        public async Task<List<SpinLogicDataModelMongo>> GetHistoryAsync(string key, int skip, int take)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("key", key);
            var result = await _busyUsersCollectionHistory.Find(filter).Skip(skip).Limit(take).ToListAsync();
            return result?.Select(doc => Deserialize(doc)).ToList() ?? new List<SpinLogicDataModelMongo>();
        }

        public async Task<SpinLogicDataModelMongo> GetAsync(string key)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("key", key);
            var result = await _busyUsersCollection.Find(filter).FirstOrDefaultAsync();
            return result != null ? Deserialize(result) : null;
        }

        public async Task<bool> UpdateAsync(string key, SpinLogicDataModelMongo updatedDataModel)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("key", key);
            var update = Builders<BsonDocument>.Update.Set("SpinLogicData", BsonDocument.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(updatedDataModel)));
            var updateResult = await _busyUsersCollection.UpdateOneAsync(filter, update);
            return updateResult.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(string key)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("key", key);
            var deleteResult = await _busyUsersCollection.DeleteOneAsync(filter);
            return deleteResult.DeletedCount > 0;
        }

        private BsonDocument CreateBsonDocument(string key, SpinLogicDataModelMongo dataModel)
        {
            return new BsonDocument
            {
                { "key", key },
                { "TimeStamp", DateTime.UtcNow },
                { "SpinLogicData", BsonDocument.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(dataModel)) }
            };
        }

        private SpinLogicDataModelMongo Deserialize(BsonDocument doc)
        {
            return BsonSerializer.Deserialize<SpinLogicDataModelMongo>(doc["SpinLogicData"].AsBsonDocument);
        }
    }
}
