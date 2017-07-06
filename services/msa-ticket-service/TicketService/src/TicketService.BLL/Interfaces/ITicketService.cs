using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketService.BLL.DTO;
using TicketService.Core.Enums;

namespace TicketService.BLL.Interfaces
{
    public interface ITicketService
    {
        IEnumerable<TicketDto> GetAll(Guid teamId);

        Task<int> GetCountAsync(Guid teamId, FilterDto filterDto);

        Task<IEnumerable<TicketDto>> GetAllFilteredAsync(Guid teamId, FilterDto filterDto);

        Task<TicketDto> GetAsync(Guid teamId, Guid id);

        Task<Guid> CreateAsync(Guid teamId, TicketDto ticketDto);

        Task UpdateAsync(Guid teamId, TicketDto ticketDto);

        Task DeleteAsync(Guid teamId, Guid id);

        Task UpdateStatusAsync(Guid teamId, Guid id, Status status);
    }
}