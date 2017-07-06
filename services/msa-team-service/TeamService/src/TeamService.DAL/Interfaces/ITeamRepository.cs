using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TeamService.DAL.Entities;

namespace TeamService.DAL.Interfaces
{
    public interface ITeamRepository
    {
        IEnumerable<Team> GetAll();

        Task<Team> GetAsync(Guid id);

        IEnumerable<Team> Find(Expression<Func<Team, bool>> expression);

        Task<Guid> CreateAsync(Team team);

        Task<Guid> UpdateAsync(Team team);

        Task DeleteAsync(Guid id);
    }
}