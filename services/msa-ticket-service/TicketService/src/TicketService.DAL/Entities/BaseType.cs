using System;
using MongoDB.Bson.Serialization.Attributes;

namespace TicketService.DAL.Entities
{
    public abstract class BaseType
    {
        [BsonId]
        public Guid Id { get; set; }

        public abstract string CollectionName { get; }
    }
}