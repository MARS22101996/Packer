namespace TicketService.DAL.Interfaces
{
    public interface IUnitOfWork
    {
        ITicketRepository Tickets { get; }

        ICommentRepository Comments { get; }

        ITagRepository Tags { get; }
    }
}