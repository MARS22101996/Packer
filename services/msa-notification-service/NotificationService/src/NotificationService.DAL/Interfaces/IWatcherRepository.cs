using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NotificationService.DAL.Entities;

namespace NotificationService.DAL.Interfaces
{
    public interface IWatcherRepository
    {
        Task<User> GetAsync(Guid teamId, Guid ticketId, Guid watcherId);

        Task<IEnumerable<User>> GetAllAsync(Guid teamId, Guid ticketId);

        Task<Guid> AddAsync(Guid ticketId, User watcher);

        Task RemoveAsync(Guid ticketId, Guid watcherId);
    }
}