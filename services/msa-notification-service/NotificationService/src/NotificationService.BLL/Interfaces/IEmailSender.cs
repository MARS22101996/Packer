using System.Collections.Generic;
using System.Threading.Tasks;

namespace NotificationService.BLL.Interfaces
{
    public interface IEmailSender
    {
        Task SendAsync(IEnumerable<string> emails, string message, string subject);
    }
}