namespace NotificationService.DAL.Entities
{
    public class User : BaseType
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public override string CollectionName => "users";
    }
}