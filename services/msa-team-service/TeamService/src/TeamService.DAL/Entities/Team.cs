using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace TeamService.DAL.Entities
{
    [BsonIgnoreExtraElements]
    public class Team : BaseType
    {
        public Team()
        {
            Participants = new List<User>();
        }

        public string Name { get; set; }

        public User Owner { get; set; }

        public IEnumerable<User> Participants { get; set; }

        public override string CollectionName => "teams";
    }
}