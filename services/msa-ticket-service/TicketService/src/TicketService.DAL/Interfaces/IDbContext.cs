using MongoDB.Bson;
using MongoDB.Driver;
using TicketService.DAL.Entities;

namespace TicketService.DAL.Interfaces
{
    public interface IDbContext
    {
        IMongoCollection<BsonDocument> GetCollection(string collectionName);

        IMongoCollection<TEntity> GetCollection<TEntity>() where TEntity : BaseType, new();
    }
}