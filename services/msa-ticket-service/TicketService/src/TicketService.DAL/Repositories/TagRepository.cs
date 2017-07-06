using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using TicketService.DAL.Interfaces;
using Tag = TicketService.DAL.Entities.Tag;

namespace TicketService.DAL.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly IDbContext _context;

        public TagRepository(IDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Tag> GetAll()
        {
            var collection = _context.GetCollection<Tag>();

            return collection.AsQueryable().ToList();
        }

        public async Task<Guid> CreateAsync(Tag item)
        {
            var collection = _context.GetCollection<Tag>();
            item.Id = Guid.NewGuid();
            await collection.InsertOneAsync(item);

            return item.Id;
        }
    }
}