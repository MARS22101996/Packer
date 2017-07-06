using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using TeamService.DAL.Entities;
using TeamService.DAL.Interfaces;

namespace TeamService.DAL.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly IDbContext _context;
        private readonly string _collectionName;

        public TeamRepository(IDbContext context)
        {
            _context = context;
            _collectionName = new Team().CollectionName;
        }

        public IEnumerable<Team> GetAll()
        {
            var collection = _context.GetCollection<Team>(_collectionName);

            return collection.AsQueryable().ToList();
        }

        public async Task<Team> GetAsync(Guid id)
        {
            var collection = _context.GetCollection<Team>(_collectionName);
            var team = (await collection.FindAsync(e => e.Id.Equals(id))).FirstOrDefault();

            return team;
        }

        public IEnumerable<Team> Find(Expression<Func<Team, bool>> expression)
        {
            var collection = _context.GetCollection<Team>(_collectionName);
            var filterCompile = expression.Compile();
            var teams = collection.AsQueryable().Where(filterCompile);

            return teams.ToList();
        }

        public async Task<Guid> CreateAsync(Team team)
        {
            var collection = _context.GetCollection<Team>(_collectionName);

            team.Id = Guid.NewGuid();

            await collection.InsertOneAsync(team);

            return team.Id;
        }

        public async Task<Guid> UpdateAsync(Team team)
        {
            var collection = _context.GetCollection<Team>(_collectionName);

            var filter = Builders<Team>.Filter.Eq(t => t.Id, team.Id);
            var teamDoc =
                new BsonDocument { { "$set", team.ToBsonDocument() } };

            await collection.UpdateOneAsync(filter, new BsonDocumentUpdateDefinition<Team>(teamDoc));

            return team.Id;
        }

        public Task DeleteAsync(Guid id)
        {
            var collection = _context.GetCollection<Team>(_collectionName);

            return collection.DeleteOneAsync(entity => entity.Id.Equals(id));
        }
    }
}