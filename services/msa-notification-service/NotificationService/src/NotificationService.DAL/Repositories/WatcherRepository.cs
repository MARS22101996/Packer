using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using NotificationService.DAL.Entities;
using NotificationService.DAL.Interfaces;

namespace NotificationService.DAL.Repositories
{
    public class WatcherRepository : IWatcherRepository
    {
        private const int FiltredElementIndex = -1;
        private readonly IDbContext _context;

        public WatcherRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetAsync(Guid teamId, Guid ticketId, Guid watcherId)
        {
            var filter = Builders<Team>.Filter.Eq("Tickets.Watchers._id", BsonBinaryData.Create(watcherId));
            var teams = await _context.GetCollection<Team>()
                .Find(filter)
                .Project<Team>(Builders<Team>.Projection.Include(t => t.Tickets))
                .ToListAsync();

            var team = teams.FirstOrDefault();
            var watchers = team?.Tickets?.FirstOrDefault(t => t.Id == ticketId)?.Watchers ?? new List<User>();

            return watchers.FirstOrDefault(user => user.Id.Equals(watcherId));
        }

        public async Task<IEnumerable<User>> GetAllAsync(Guid teamId, Guid ticketId)
        {
            var filter = Builders<Team>.Filter.Eq("_id", BsonBinaryData.Create(teamId));
            var teams = await _context.GetCollection<Team>()
                .Find(filter)
                .Project<Team>(Builders<Team>.Projection.Include(t => t.Tickets))
                .ToListAsync();

            var team = teams.FirstOrDefault();
            var watchers = team?.Tickets?.FirstOrDefault(t => t.Id == ticketId)?.Watchers ?? new List<User>();

            return watchers;
        }

        public async Task<Guid> AddAsync(Guid ticketId, User watcher)
        {
            var filter = Builders<Team>.Filter
                .Eq("Tickets._id", BsonBinaryData.Create(ticketId));
            var update = Builders<Team>.Update
                .Push(t => t.Tickets.ElementAt(FiltredElementIndex).Watchers, watcher);

            await _context.GetCollection<Team>().UpdateOneAsync(filter, update);

            return watcher.Id;
        }

        public async Task RemoveAsync(Guid ticketId, Guid watcherId)
        {
            var update = Builders<Team>.Update.PullFilter(p => p.Tickets.ElementAt(-1).Watchers, f => f.Id.Equals(watcherId));
            var filter = Builders<Team>.Filter.Eq("Tickets.Watchers._id", BsonBinaryData.Create(watcherId));

            await _context.GetCollection<Team>().FindOneAndUpdateAsync(filter, update);
        }
    }
}