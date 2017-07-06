using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace NotificationService.DAL.Entities
{
    [BsonIgnoreExtraElements]
    internal class Ticket : BaseType
    {
        public IEnumerable<User> Watchers { get; set; }

        public override string CollectionName => "tickets";
    }
}