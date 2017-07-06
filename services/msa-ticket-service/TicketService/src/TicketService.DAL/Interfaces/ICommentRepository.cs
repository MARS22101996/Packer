using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketService.DAL.Entities;

namespace TicketService.DAL.Interfaces
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetAllAsync(Guid teamId, Guid ticketId);

        Task<Guid> CreateAsync(Guid teamId, Guid ticketId, Comment item);

        Task DeleteAsync(Guid teamId, Guid ticketId, Guid id);
    }
}