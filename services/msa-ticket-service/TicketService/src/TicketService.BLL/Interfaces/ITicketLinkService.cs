using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketService.BLL.DTO;

namespace TicketService.BLL.Interfaces
{
    public interface ITicketLinkService
    {
        Task<IEnumerable<TicketDto>> GetLinkedTicketsAsync(Guid teamId, Guid id);

        Task<IEnumerable<TicketDto>> GetUnlinkedTicketsAsync(Guid teamId, Guid id);

        Task UnlinkTicketsAsync(Guid teamId, Guid sourceTicketId, Guid destinationTicketId);

        Task LinkTicketsAsync(Guid teamId, Guid sourceTicketId, Guid destinationTicketId);
    }
}