using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketService.BLL.DTO;

namespace TicketService.BLL.Interfaces
{
    public interface ICommentService
    {
        Task<IEnumerable<CommentDto>> GetAllAsync(Guid teamId, Guid ticketId);

        Task<Guid> CreateAsync(Guid teamId, Guid ticketId, CommentDto commentDto);

        Task DeleteAsync(Guid teamId, Guid ticketId, Guid id);
    }
}