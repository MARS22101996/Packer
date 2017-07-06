using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace NotificationService.DAL.Entities
{
    [BsonIgnoreExtraElements]
    internal class Team : BaseType
    {
        public IEnumerable<Ticket> Tickets { get; set; }

        public override string CollectionName => "teams";
    }
}