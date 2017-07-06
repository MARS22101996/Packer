using System;

namespace TicketService.BLL.DTO
{
    public class CommentDto
    {
        public Guid Id { get; set; }

        public string Text { get; set; }

        public DateTime Date { get; set; }
        
        public UserDto User { get; set; }
    }
}