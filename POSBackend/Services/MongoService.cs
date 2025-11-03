using MongoDB.Bson;
using MongoDB.Driver;

namespace POSBackend.Services
{
    public class MongoService
    {
        private readonly IMongoDatabase _database;

        public MongoService(string connectionString, string dbName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(dbName);
        }

        public IMongoCollection<BsonDocument> Products => _database.GetCollection<BsonDocument>("products");
    }
}
