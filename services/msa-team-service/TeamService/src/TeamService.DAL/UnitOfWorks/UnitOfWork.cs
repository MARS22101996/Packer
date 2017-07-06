using System;
using TeamService.DAL.Interfaces;
using TeamService.DAL.Repositories;

namespace TeamService.DAL.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Lazy<ITeamRepository> _teamRepository;

        public UnitOfWork(IDbContext context)
        {
            var db = context;
            _teamRepository = new Lazy<ITeamRepository>(() => new TeamRepository(db));
        }

        public ITeamRepository Teams => _teamRepository.Value;
    }
}