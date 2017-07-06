using System;
using NotificationService.BLL.Infrastructure.Exceptions;
using NotificationService.Core.Enums;

namespace NotificationService.BLL.Infrastructure
{
    public static class MessageFormater
    {
        public static string FormNotificationMassage(NotificationInfo info)
        {
            try
            {
                switch (info.NotificationType)
                {
                    case NotificationType.StatusUpdated:
                        return StatusUpdatedMessage(info);
                    case NotificationType.TicketDeleted:
                        return TicketDeletedMessage(info);
                    case NotificationType.AssigneeChanged:
                        return AssigneeChangedMessage(info);
                    case NotificationType.TicketUpdated:
                        return TicketUpdatedMessage(info);
                }
            }
            catch (NullReferenceException)
            {
                throw new ServiceException(
                    "You need to provide enough information about ticket for notification." +
                    "NewTicket and OldTicket are required (on ticket deleting only OldTicket)", 
                    "NotificationInfo");
            }

            return string.Empty;
        }

        private static string StatusUpdatedMessage(NotificationInfo info)
        {
            var message = $"Ticket name: {info.OldTicket.Name} (id: {info.OldTicket.Id})\n\n" +
                          $"Status changed from '{info.OldTicket.Status}' to '{info.NewTicket?.Status}'";

            return message;
        }

        private static string TicketUpdatedMessage(NotificationInfo info)
        {
            var message = $"Ticket name: {info.OldTicket.Name} (id: {info.OldTicket.Id})\n\n" +
                          $"New ticket info \n\tName: {info.NewTicket?.Name}\n\tText: {info.NewTicket?.Text}\n\t" +
                          $"Priority: {info.NewTicket?.Priority}\n\tStatus: {info.NewTicket?.Status}\n\n" +
                          $"Old ticket info \n\tName: {info.OldTicket.Name}\n\tText: {info.OldTicket.Text}\n\t" +
                          $"Priority: {info.OldTicket.Priority}\n\tStatus: {info.OldTicket.Status}";

            return message;
        }

        private static string AssigneeChangedMessage(NotificationInfo info)
        {
            var assigneeInfo = info.NewTicket?.Assignee == null || info.NewTicket?.Assignee.Id == Guid.Empty
                ? "Assignee was removed"
                : $"Assigned to: {info.NewTicket?.Assignee.Email}";

            return $"Ticket name: {info.OldTicket.Name} (id: {info.OldTicket.Id})\n\n{assigneeInfo}";
        }

        private static string TicketDeletedMessage(NotificationInfo info)
        {
            return $"Ticket name: {info.OldTicket.Name} (id: {info.OldTicket.Id})\n\nTicket was deleted.";
        }
    }
}