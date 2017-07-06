using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using TicketService.DAL.Entities;
using TicketService.DAL.Interfaces;

namespace TicketService.DAL.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private const int FilteredElementIndex = -1;

        private readonly IDbContext _context;
  
        public TicketRepository(IDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Ticket>> GetAll(Guid teamId)
        {
            var filter = Builders<Team>.Filter.Eq("_id", BsonBinaryData.Create(teamId));
            var teams = await _context.GetCollection<Team>()
                .Find(filter)
                .Project<Team>(Builders<Team>.Projection.Include(t => t.Tickets))
                .ToListAsync();

            var team = teams.FirstOrDefault();

            return team == null ? new List<Ticket>() : team.Tickets;
        }

        public async Task<Ticket> GetAsync(Guid teamId, Guid id)
        {
            var filter = Builders<Team>.Filter.Eq("_id", BsonBinaryData.Create(teamId));
            var teams = await _context.GetCollection<Team>()
                .Find(filter)
                .Project<Team>(Builders<Team>.Projection.Include(t => t.Tickets))
                .ToListAsync();

            var team = teams.FirstOrDefault();

            return team?.Tickets?.FirstOrDefault(ticket => ticket.Id == id);
        }

        public async Task<IEnumerable<Ticket>> FindAsync(Guid teamId, Expression<Func<Ticket, bool>> expression)
        {
            var filter = Builders<Team>.Filter.Eq("_id", BsonBinaryData.Create(teamId));
            var teams = await _context.GetCollection<Team>()
                .Find(filter)
                .Project<Team>(Builders<Team>.Projection.Include(t => t.Tickets))
                .ToListAsync();

            var team = teams.FirstOrDefault();
            var tickets = team == null
                ? new List<Ticket>()
                : team.Tickets.Where(expression.Compile());

            return tickets;
        }

        public async Task<IEnumerable<Ticket>> FindAsync(Guid teamId, int skip, int take, Expression<Func<Ticket, bool>> expression = null)
        {
            var filter = Builders<Team>.Filter.Eq("_id", BsonBinaryData.Create(teamId));
            var teams = await _context.GetCollection<Team>()
                .Find(filter)
                .Project<Team>(Builders<Team>.Projection.Include(t => t.Tickets))
                .ToListAsync();

            var team = teams.FirstOrDefault();
            var tickets = team?.Tickets ?? new List<Ticket>();

            if (expression != null)
            {
                tickets = tickets.Where(expression.Compile()).ToList();
            }
            
            return tickets.Skip(skip).Take(take);
        }

        public async Task<Guid> CreateAsync(Guid teamId, Ticket item)
        {
            item.Id = Guid.NewGuid();
            item.Comments = new List<Comment>();
            var filter = Builders<Team>.Filter.Eq("_id", BsonBinaryData.Create(teamId));
            var update = Builders<Team>.Update.Push(t => t.Tickets, item);

            await _context.GetCollection<Team>().UpdateOneAsync(filter, update);

            return item.Id;
        }

        public async Task<int> GetCountAsync(Guid teamId, Expression<Func<Ticket, bool>> expression = null)
        {
            var filter = Builders<Team>.Filter.Eq("_id", BsonBinaryData.Create(teamId));
            var teams = await _context.GetCollection<Team>()
                .Find(filter)
                .Project<Team>(Builders<Team>.Projection.Include(t => t.Tickets))
                .ToListAsync();

            var team = teams.FirstOrDefault();
            var tickets = team == null ? new List<Ticket>() : team.Tickets;

            if (expression != null)
            {
                tickets = tickets.Where(expression.Compile()).ToList();
            }

            return tickets.Count;
        }

        public async Task<Guid> UpdateAsync(Guid teamId, Ticket item)
        {
            var filter = Builders<Team>.Filter.Eq("Tickets._id", BsonBinaryData.Create(item.Id));

            var update = Builders<Team>.Update
                .Set(x => x.Tickets[FilteredElementIndex].Name, item.Name)
                .Set(x => x.Tickets[FilteredElementIndex].Text, item.Text)
                .Set(x => x.Tickets[FilteredElementIndex].Tags, item.Tags)
                .Set(x => x.Tickets[FilteredElementIndex].Priority, item.Priority)
                .Set(x => x.Tickets[FilteredElementIndex].Status, item.Status)
                .Set(x => x.Tickets[FilteredElementIndex].Assignee, item.Assignee)
                .Set(x => x.Tickets[FilteredElementIndex].CreationDate, item.CreationDate)
                .Set(x => x.Tickets[FilteredElementIndex].LinkedTicketIds, item.LinkedTicketIds);

            await _context.GetCollection<Team>().UpdateOneAsync(filter, update);

            return item.Id;
        }

        public async Task DeleteAsync(Guid teamId, Guid id)
        {
            var update = Builders<Team>.Update.PullFilter(p => p.Tickets, f => f.Id.Equals(id));
            var filter = Builders<Team>.Filter.Eq("Tickets._id", BsonBinaryData.Create(id));

            await _context.GetCollection<Team>().FindOneAndUpdateAsync(filter, update);
        }
    }
}