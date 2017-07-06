using System;
using NotificationService.DAL.Interfaces;

namespace NotificationService.DAL.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Lazy<IWatcherRepository> _watcherRepository;

        public UnitOfWork(IWatcherRepository watcherRepository)
        {
            _watcherRepository = new Lazy<IWatcherRepository>(() => watcherRepository);
        }

        public IWatcherRepository Watchers => _watcherRepository.Value;
    }
}