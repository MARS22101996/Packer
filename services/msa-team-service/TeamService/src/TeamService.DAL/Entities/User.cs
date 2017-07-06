using MongoDB.Bson.Serialization.Attributes;

namespace TeamService.DAL.Entities
{
    [BsonIgnoreExtraElements]
    public class User : BaseType
    {
        public string Email { get; set; }

        public string Name { get; set; }

        public override string CollectionName => "users";
    }
}