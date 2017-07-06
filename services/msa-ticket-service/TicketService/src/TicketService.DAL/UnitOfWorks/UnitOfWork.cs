using System;
using TicketService.DAL.Interfaces;
using TicketService.DAL.Repositories;

namespace TicketService.DAL.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Lazy<ITicketRepository> _ticketRepository;

        private readonly Lazy<ICommentRepository> _commentRepository;

        private readonly Lazy<ITagRepository> _tagRepository;

        public UnitOfWork(IDbContext context)
        {
            var db = context;
            _ticketRepository = new Lazy<ITicketRepository>(() => new TicketRepository(db));
            _tagRepository = new Lazy<ITagRepository>(() => new TagRepository(db));
            _commentRepository = new Lazy<ICommentRepository>(() => new CommentRepository(db));
        }

        public ITicketRepository Tickets => _ticketRepository.Value;

        public ICommentRepository Comments => _commentRepository.Value;

        public ITagRepository Tags => _tagRepository.Value;
    }
}