using MongoDB.Driver;
using TeamService.DAL.Entities;

namespace TeamService.DAL.Interfaces
{
    public interface IDbContext
    {
        IMongoCollection<TEntity> GetCollection<TEntity>(string collectionName) where TEntity : BaseType;
    }
}