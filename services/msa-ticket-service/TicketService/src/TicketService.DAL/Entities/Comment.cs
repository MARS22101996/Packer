using System;

namespace TicketService.DAL.Entities
{
    public class Comment : BaseType
    {
        public string Text { get; set; }

        public DateTime Date { get; set; }

        public User User { get; set; }

        public override string CollectionName => "comments";
    }
}
