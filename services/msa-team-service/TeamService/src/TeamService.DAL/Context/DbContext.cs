using MongoDB.Driver;
using TeamService.DAL.Entities;
using TeamService.DAL.Interfaces;

namespace TeamService.DAL.Context
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

        public IMongoCollection<TEntity> GetCollection<TEntity>(string collectionName)
            where TEntity : BaseType
        {
            return _database.GetCollection<TEntity>(collectionName);
        }
    }
}