using MongoDB.Driver;
using MyApiProject.Models;

namespace MyApiProject.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly string _collectionName; // Add this line

        public MongoDbContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration["MongoDB:ConnectionString"]);
            _database = client.GetDatabase(configuration["MongoDB:DatabaseName"]);
            _collectionName = configuration["MongoDB:CollectionName"]; // Initialize the field here
        }

        public IMongoCollection<MyItem> Items => _database.GetCollection<MyItem>(_collectionName); // Use the field here
    }
}