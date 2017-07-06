using MongoDB.Bson;
using MongoDB.Driver;
using TicketService.DAL.Entities;
using TicketService.DAL.Interfaces;

namespace TicketService.DAL.Context
{
    public class DbContext : IDbContext
    {
        private readonly IMongoDatabase _database;

        public DbContext()
        {
        }

        public DbContext(string connectionString)
        {
            var builder = new MongoUrlBuilder(connectionString);

            IMongoClient client = new MongoClient(connectionString);
            _database = client.GetDatabase(builder.DatabaseName);
        }

        public IMongoCollection<BsonDocument> GetCollection(string collectionName)
        {
            return _database.GetCollection<BsonDocument>(collectionName);
        }

        public IMongoCollection<TEntity> GetCollection<TEntity>() where TEntity : BaseType, new()
        {
            return _database.GetCollection<TEntity>(new TEntity().CollectionName);
        }
    }
}