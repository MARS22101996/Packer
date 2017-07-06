using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketService.DAL.Entities;

namespace TicketService.DAL.Interfaces
{
    public interface ITagRepository
    {
        IEnumerable<Tag> GetAll();

        Task<Guid> CreateAsync(Tag item);
    }
}