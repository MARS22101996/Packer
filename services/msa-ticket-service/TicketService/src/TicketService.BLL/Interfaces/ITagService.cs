using System.Collections.Generic;
using System.Threading.Tasks;
using TicketService.BLL.DTO;

namespace TicketService.BLL.Interfaces
{
    public interface ITagService
    {
        IEnumerable<TagDto> GetAll();

        Task AddAsync(IEnumerable<string> tags);
    }
}