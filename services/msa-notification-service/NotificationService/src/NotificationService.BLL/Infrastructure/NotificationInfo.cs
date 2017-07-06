using NotificationService.BLL.DTO;
using NotificationService.Core.Enums;

namespace NotificationService.BLL.Infrastructure
{
    public class NotificationInfo
    {
        public NotificationType NotificationType { get; set; }

        public TicketDto NewTicket { get; set; }

        public TicketDto OldTicket { get; set; }
    }
}