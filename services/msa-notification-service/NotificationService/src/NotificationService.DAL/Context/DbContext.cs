using MongoDB.Bson;
using MongoDB.Driver;
using NotificationService.DAL.Entities;
using NotificationService.DAL.Interfaces;

namespace NotificationService.DAL.Context
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

        public IMongoCollection<TEntity> GetCollection<TEntity>()
            where TEntity : BaseType, new()
        {
            return _database.GetCollection<TEntity>(new TEntity().CollectionName);
        }
    }
}