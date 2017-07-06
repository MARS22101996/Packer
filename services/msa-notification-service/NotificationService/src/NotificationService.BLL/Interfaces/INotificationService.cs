using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NotificationService.BLL.DTO;
using NotificationService.BLL.Infrastructure;

namespace NotificationService.BLL.Interfaces
{
    public interface INotificationService
    {
        Task<UserDto> GetWatcherAsync(Guid teamId, Guid ticketId, Guid userId);

        Task<IEnumerable<UserDto>> GetAllWatchersAsync(Guid teamId, Guid ticketId);

        Task AddWatcherAsync(Guid ticketId, UserDto userDto);

        Task RemoveWatcherAsync(Guid teamId, Guid ticketId, Guid userId);

        Task NotifyTicketWatchersAsync(Guid teamId, Guid ticketId, NotificationInfo info);
    }
}