using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TicketService.DAL.Entities;

namespace TicketService.DAL.Interfaces
{
    public interface ITicketRepository
    {
        Task<IEnumerable<Ticket>> GetAll(Guid teamId);

        Task<Ticket> GetAsync(Guid teamId, Guid id);

        Task<IEnumerable<Ticket>> FindAsync(Guid teamId, Expression<Func<Ticket, bool>> expression);

        Task<IEnumerable<Ticket>> FindAsync(Guid teamId, int skip, int take, Expression<Func<Ticket, bool>> expression = null);

        Task<Guid> CreateAsync(Guid teamId, Ticket item);

        Task<int> GetCountAsync(Guid teamId, Expression<Func<Ticket, bool>> expression = null);

        Task<Guid> UpdateAsync(Guid teamId, Ticket item);

        Task DeleteAsync(Guid teamId, Guid id);
    }
}