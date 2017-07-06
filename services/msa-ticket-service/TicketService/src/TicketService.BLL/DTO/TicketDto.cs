using System;
using System.Collections.Generic;
using TicketService.Core.Enums;

namespace TicketService.BLL.DTO
{
    public class TicketDto
    {
        public TicketDto()
        {
            LinkedTicketIds = new List<Guid>();
            Tags = new List<string>();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public UserDto Assignee { get; set; }

        public Status Status { get; set; }

        public Priority Priority { get; set; }

        public DateTime CreationDate { get; set; }

        public IEnumerable<Guid> LinkedTicketIds { get; set; }

        public int CommentCount { get; set; }
        
        public IEnumerable<string> Tags { get; set; }
    }
}