using MongoDB.Driver;
using NotificationService.DAL.Entities;

namespace NotificationService.DAL.Interfaces
{
    public interface IDbContext
    {
        IMongoCollection<TEntity> GetCollection<TEntity>() where TEntity : BaseType, new();
    }
}