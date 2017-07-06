using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace TicketService.DAL.Entities
{
    [BsonIgnoreExtraElements]
    public class Team : BaseType
    {
        public List<Ticket> Tickets { get; set; }

        public override string CollectionName => "teams";
    }
}
