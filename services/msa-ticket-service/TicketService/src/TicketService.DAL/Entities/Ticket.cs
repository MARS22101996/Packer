using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TicketService.Core.Enums;

namespace TicketService.DAL.Entities
{
    [BsonIgnoreExtraElements]
    public class Ticket : BaseType
    {
        public Ticket()
        {
            LinkedTicketIds = new List<Guid>();
        }

        public string Name { get; set; }

        public string Text { get; set; }

        public User Assignee { get; set; }

        [BsonDateTimeOptions(Representation = BsonType.DateTime)]
        public DateTime CreationDate { get; set; }

        public Status Status { get; set; }
        
        public Priority Priority { get; set; }

        public IEnumerable<Guid> LinkedTicketIds { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public IEnumerable<Comment> Comments { get; set; }

        public override string CollectionName => "tickets";
    }
}