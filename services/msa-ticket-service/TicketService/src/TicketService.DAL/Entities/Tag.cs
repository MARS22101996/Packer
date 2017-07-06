namespace TicketService.DAL.Entities
{
    public class Tag : BaseType
    {
        public string Name { get; set; }

        public override string CollectionName => "tags";
    }
}