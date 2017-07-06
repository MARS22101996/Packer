using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using TicketService.DAL.Entities;
using TicketService.DAL.Interfaces;

namespace TicketService.DAL.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        protected readonly IDbContext Context;
        private const int FilteredElementIndex = -1;

        public CommentRepository(IDbContext context)
        {
            Context = context;
        }

        public async Task<IEnumerable<Comment>> GetAllAsync(Guid teamId, Guid ticketId)
        {
            var filter = Builders<Team>.Filter.Eq("_id", BsonBinaryData.Create(teamId));
            var teams = await Context.GetCollection<Team>()
                .Find(filter)
                .Project<Team>(Builders<Team>.Projection.Include(t => t.Tickets))
                .ToListAsync();

            var team = teams.FirstOrDefault();
            var result = team?.Tickets?.FirstOrDefault(t => t.Id == ticketId)?.Comments ?? new List<Comment>();

            return result;
        }

        public async Task<Guid> CreateAsync(Guid teamId, Guid ticketId, Comment item)
        {
            item.Id = Guid.NewGuid();
            var filter = Builders<Team>.Filter.Eq("Tickets._id", BsonBinaryData.Create(ticketId));
            var update = Builders<Team>.Update.Push(t => t.Tickets[FilteredElementIndex].Comments, item);

            await Context.GetCollection<Team>().UpdateOneAsync(filter, update);

            return item.Id;
        }

        public async Task DeleteAsync(Guid teamId, Guid ticketId, Guid id)
        {
            var update = Builders<Team>.Update.PullFilter(p => p.Tickets[FilteredElementIndex].Comments, f => f.Id.Equals(id));
            var filter = Builders<Team>.Filter.Eq("Tickets.Comments._id", BsonBinaryData.Create(id));

            await Context.GetCollection<Team>().FindOneAndUpdateAsync(filter, update);
        }
    }
}